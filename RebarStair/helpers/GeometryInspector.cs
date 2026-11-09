using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Globalization;
using System.IO;
using System.Text;

namespace RebarStair.helpers
{
    internal static class GeometryInspector
    {
        private const int MaxLoggedFaces = 500;
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RebarStair");

        internal static string LogPath => Path.Combine(LogDirectory, "geometry-inspection.log");

        internal static string Inspect(Document document, Element element)
        {
            var text = new StringBuilder();
            text.AppendLine(new string('=', 96));
            text.AppendLine($"Timestamp: {DateTimeOffset.Now:O}");
            text.AppendLine($"Document: {document.Title}");
            text.AppendLine($"ElementId: {element.Id.Value}");
            text.AppendLine($"RuntimeType: {element.GetType().FullName}");
            text.AppendLine($"Category: {element.Category?.Name ?? "<none>"}");
            text.AppendLine($"Name: {element.Name}");
            text.AppendLine($"RebarHost: {RebarHostData.IsValidHost(element)}");
            WriteBoundingBox(text, "ElementBoundingBox", element.get_BoundingBox(null), Transform.Identity);
            WriteParameters(text, element);

            var counters = new GeometryCounters();
            try
            {
                var geometry = element.get_Geometry(new Options
                {
                    ComputeReferences = true,
                    IncludeNonVisibleObjects = true,
                    DetailLevel = ViewDetailLevel.Fine
                });
                if (geometry == null)
                {
                    text.AppendLine("GeometryElement: <null>");
                }
                else
                {
                    InspectGeometry(text, geometry, Transform.Identity, 0, counters);
                }
            }
            catch (Exception exception)
            {
                text.AppendLine($"Geometry inspection failed: {exception}");
            }

            text.AppendLine(
                $"Summary: Instances={counters.Instances}, Solids={counters.Solids}, " +
                $"Faces={counters.Faces}, PlanarFaces={counters.PlanarFaces}, " +
                $"Meshes={counters.Meshes}, Triangles={counters.Triangles}, Curves={counters.Curves}");
            // shortcut: cap detailed face rows to keep imported-mesh logs usable; raise the cap for forensic dumps.
            if (counters.Faces > MaxLoggedFaces)
            {
                text.AppendLine($"Detailed face rows truncated at {MaxLoggedFaces}.");
            }

            Directory.CreateDirectory(LogDirectory);
            File.AppendAllText(LogPath, text.ToString(), Encoding.UTF8);
            return LogPath;
        }

        internal static void LogException(string operation, Exception exception)
        {
            Directory.CreateDirectory(LogDirectory);
            File.AppendAllText(
                LogPath,
                $"{Environment.NewLine}{DateTimeOffset.Now:O} [{operation}]{Environment.NewLine}{exception}{Environment.NewLine}",
                Encoding.UTF8);
        }

        internal static void LogEvent(string operation, string details)
        {
            Directory.CreateDirectory(LogDirectory);
            File.AppendAllText(
                LogPath,
                $"{Environment.NewLine}{DateTimeOffset.Now:O} [{operation}] {details}{Environment.NewLine}",
                Encoding.UTF8);
        }

        private static void WriteParameters(StringBuilder text, Element element)
        {
            text.AppendLine($"Parameters: {element.Parameters.Size}");
            foreach (Parameter parameter in element.Parameters)
            {
                string value;
                string displayValue;
                try
                {
                    value = parameter.StorageType switch
                    {
                        StorageType.Double => parameter.AsDouble().ToString("G17", CultureInfo.InvariantCulture),
                        StorageType.Integer => parameter.AsInteger().ToString(CultureInfo.InvariantCulture),
                        StorageType.String => parameter.AsString() ?? "<null>",
                        StorageType.ElementId => parameter.AsElementId().Value.ToString(CultureInfo.InvariantCulture),
                        _ => "<none>"
                    };
                    displayValue = parameter.AsValueString() ?? "<none>";
                }
                catch (Exception exception)
                {
                    value = $"<error: {exception.Message}>";
                    displayValue = "<error>";
                }

                text.AppendLine(
                    $"  [{parameter.StorageType}] {parameter.Definition?.Name ?? "<unnamed>"} = " +
                    $"{value} | Display={displayValue}");
            }
        }

        private static void InspectGeometry(
            StringBuilder text,
            GeometryElement geometry,
            Transform transform,
            int depth,
            GeometryCounters counters)
        {
            var indent = new string(' ', depth * 2);
            foreach (var geometryObject in geometry)
            {
                try
                {
                    switch (geometryObject)
                    {
                        case GeometryInstance instance:
                            counters.Instances++;
                            var instanceTransform = transform.Multiply(instance.Transform);
                            text.AppendLine($"{indent}GeometryInstance #{counters.Instances}");
                            WriteTransform(text, indent + "  ", instanceTransform);
                            InspectGeometry(text, instance.GetSymbolGeometry(), instanceTransform, depth + 1, counters);
                            break;

                        case Solid solid when solid.Faces.Size > 0:
                            counters.Solids++;
                            text.AppendLine(
                                $"{indent}Solid #{counters.Solids}: Volume={solid.Volume:G17}, " +
                                $"Faces={solid.Faces.Size}, Edges={solid.Edges.Size}");
                            foreach (Face face in solid.Faces)
                            {
                                counters.Faces++;
                                if (face is PlanarFace planarFace) counters.PlanarFaces++;
                                if (counters.Faces <= MaxLoggedFaces)
                                {
                                    WriteFace(text, indent + "  ", face, transform, counters.Faces);
                                }
                            }
                            break;

                        case Mesh mesh:
                            counters.Meshes++;
                            counters.Triangles += mesh.NumTriangles;
                            text.AppendLine(
                                $"{indent}Mesh #{counters.Meshes}: Vertices={mesh.Vertices.Count}, " +
                                $"Triangles={mesh.NumTriangles}");
                            WriteMeshBounds(text, indent + "  ", mesh, transform);
                            break;

                        case Curve curve:
                            counters.Curves++;
                            var start = transform.OfPoint(curve.GetEndPoint(0));
                            var end = transform.OfPoint(curve.GetEndPoint(1));
                            text.AppendLine(
                                $"{indent}Curve #{counters.Curves}: {curve.GetType().Name}, " +
                                $"Start={Format(start)}, End={Format(end)}, Length={curve.Length:G17}");
                            break;

                        default:
                            text.AppendLine($"{indent}{geometryObject.GetType().Name}");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    text.AppendLine($"{indent}{geometryObject.GetType().Name} inspection error: {exception.Message}");
                }
            }
        }

        private static void WriteFace(StringBuilder text, string indent, Face face, Transform transform, int index)
        {
            if (face is PlanarFace planarFace)
            {
                var normal = transform.OfVector(planarFace.FaceNormal).Normalize();
                var origin = transform.OfPoint(planarFace.Origin);
                text.AppendLine(
                    $"{indent}Face #{index}: Planar, Area={face.Area:G17}, " +
                    $"Normal={Format(normal)}, Origin={Format(origin)}");
                return;
            }

            text.AppendLine($"{indent}Face #{index}: {face.GetType().Name}, Area={face.Area:G17}");
        }

        private static void WriteBoundingBox(
            StringBuilder text,
            string label,
            BoundingBoxXYZ? bounds,
            Transform transform)
        {
            if (bounds == null)
            {
                text.AppendLine($"{label}: <null>");
                return;
            }

            var combined = transform.Multiply(bounds.Transform);
            text.AppendLine(
                $"{label}: Min={Format(combined.OfPoint(bounds.Min))}, " +
                $"Max={Format(combined.OfPoint(bounds.Max))}");
        }

        private static void WriteMeshBounds(StringBuilder text, string indent, Mesh mesh, Transform transform)
        {
            if (mesh.Vertices.Count == 0) return;
            var points = mesh.Vertices.Select(transform.OfPoint).ToList();
            var min = new XYZ(points.Min(point => point.X), points.Min(point => point.Y), points.Min(point => point.Z));
            var max = new XYZ(points.Max(point => point.X), points.Max(point => point.Y), points.Max(point => point.Z));
            text.AppendLine($"{indent}Bounds: Min={Format(min)}, Max={Format(max)}");
        }

        private static void WriteTransform(StringBuilder text, string indent, Transform transform)
        {
            text.AppendLine(
                $"{indent}Transform: Origin={Format(transform.Origin)}, X={Format(transform.BasisX)}, " +
                $"Y={Format(transform.BasisY)}, Z={Format(transform.BasisZ)}");
        }

        private static string Format(XYZ point) => string.Create(
            CultureInfo.InvariantCulture,
            $"({point.X:G17}, {point.Y:G17}, {point.Z:G17})");

        private sealed class GeometryCounters
        {
            internal int Instances { get; set; }
            internal int Solids { get; set; }
            internal int Faces { get; set; }
            internal int PlanarFaces { get; set; }
            internal int Meshes { get; set; }
            internal int Triangles { get; set; }
            internal int Curves { get; set; }
        }
    }
}
