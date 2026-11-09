using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RebarStair.helpers;
using RebarStair.RebarStair.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.RebarStair.Services
{
    /// <summary>Dựng dữ liệu mặt cắt 2D tại work plane đã chọn.</summary>
    public interface ISectionService
    {
        bool HasPlane { get; }

        /// <summary>Đặt mặt cắt theo mặt phẳng vừa pick, tự lùi vào trong thân thang.</summary>
        void SetFace(PlanarFace face);

        /// <summary>Trả về dữ liệu mặt cắt, null nếu chưa có work plane hoặc không cắt được vế thang.</summary>
        SectionDrawing? Build(Element stair, IReadOnlyList<ElementId> rebarIds);
    }

    /// <summary>
    /// Cắt solid của vế thang bằng một lát mỏng trên mặt phẳng work plane (lùi vào trong
    /// thân thang một đoạn) rồi chiếu biên dạng + đường thép lên mặt phẳng 2D.
    /// </summary>
    public sealed class SectionService : ISectionService
    {
        private const double CanvasSize = 280;
        private const double CanvasMargin = 12;

        /// <summary>Lùi mặt cắt vào trong thân thang so với mặt work plane đã pick (mm).</summary>
        private const double InsetDepthMm = 25;

        /// <summary>Độ dày lát cắt dùng cho phép giao solid (mm).</summary>
        private const double SlabThicknessMm = 10;

        private readonly Document _document;
        private Plane? _plane;
        private Plane? _outlinePlane;
        private List<XYZ>? _outlineCache;

        public SectionService(Document document) =>
            _document = document ?? throw new ArgumentNullException(nameof(document));

        public bool HasPlane => _plane != null;

        public void SetFace(PlanarFace face)
        {
            var normal = face.FaceNormal.Normalize();
            var origin = face.Origin - normal * InsetDepthMm.MmToFeet();
            _plane = Plane.CreateByNormalAndOrigin(normal, origin);
        }

        public SectionDrawing? Build(Element stair, IReadOnlyList<ElementId> rebarIds)
        {
            if (_plane is null || stair is null) return null;

            try
            {
                if (!ReferenceEquals(_outlinePlane, _plane) || _outlineCache is null)
                {
                    _outlinePlane = _plane;
                    _outlineCache = ComputeSectionOutline(stair, _plane) ?? GetFallbackOutline(stair);
                }

                var outline = _outlineCache;
                if (outline is null || outline.Count < 3) return null;

                var vertical = XYZ.BasisZ;
                var normal = GetOutlineNormal(outline);
                var horizontal = normal.CrossProduct(XYZ.BasisZ);
                horizontal = horizontal.GetLength() < 1e-6 ? XYZ.BasisX : horizontal.Normalize();

                var minU = outline.Min(point => point.DotProduct(horizontal));
                var maxU = outline.Max(point => point.DotProduct(horizontal));
                var minV = outline.Min(point => point.DotProduct(vertical));
                var maxV = outline.Max(point => point.DotProduct(vertical));

                var scale = CanvasSize / Math.Max(maxU - minU, 1e-6);
                var width = (maxU - minU) * scale + 2 * CanvasMargin;
                var height = (maxV - minV) * scale + 2 * CanvasMargin;

                Point2D ToCanvas(XYZ point) => new(
                    (point.DotProduct(horizontal) - minU) * scale + CanvasMargin,
                    (maxV - point.DotProduct(vertical)) * scale + CanvasMargin);

                var outlinePoints = outline.Select(ToCanvas).ToList();
                var rebarPolylines = BuildRebarPolylines(rebarIds, ToCanvas);

                var widthMm = ((width - 2 * CanvasMargin) / scale).FeetToMm();
                var heightMm = ((height - 2 * CanvasMargin) / scale).FeetToMm();
                return new SectionDrawing(
                    outlinePoints,
                    rebarPolylines,
                    width,
                    height,
                    $"{widthMm:0} × {heightMm:0} mm");
            }
            catch (Exception exception)
            {
                GeometryInspector.LogException("Build 2D section", exception);
                return null;
            }
        }

        private List<IReadOnlyList<Point2D>> BuildRebarPolylines(
            IReadOnlyList<ElementId> rebarIds,
            Func<XYZ, Point2D> toCanvas)
        {
            var polylines = new List<IReadOnlyList<Point2D>>();
            foreach (var id in rebarIds)
            {
                if (_document.GetElement(id) is not Autodesk.Revit.DB.Structure.Rebar rebar) continue;

                IList<Curve> curves;
                try
                {
                    curves = rebar.GetCenterlineCurves(
                        false,
                        false,
                        false,
                        MultiplanarOption.IncludeOnlyPlanarCurves,
                        0);
                }
                catch (Exception exception)
                {
                    GeometryInspector.LogException("Section rebar curves", exception);
                    continue;
                }

                foreach (var curve in curves)
                {
                    var points = curve.Tessellate();
                    if (points.Count < 2) continue;
                    polylines.Add(points.Select(toCanvas).ToList());
                }
            }

            return polylines;
        }

        /// <summary>Cắt solid của vế thang bằng một lát mỏng quanh mặt phẳng work plane.</summary>
        private List<XYZ>? ComputeSectionOutline(Element stair, Plane plane)
        {
            var solid = GetStairSolid(stair);
            if (solid == null) return null;

            var slab = CreateSectionSlab(stair, plane);
            if (slab == null) return null;

            Solid? cut;
            try
            {
                cut = BooleanOperationsUtils.ExecuteBooleanOperation(solid, slab, BooleanOperationsType.Intersect);
            }
            catch (Exception exception)
            {
                GeometryInspector.LogException("Section boolean", exception);
                return null;
            }

            if (cut == null || cut.Faces.Size == 0) return null;

            var normal = plane.Normal;
            PlanarFace? sectionFace = null;
            var bestArea = 0d;
            foreach (Face face in cut.Faces)
            {
                if (face is PlanarFace planar &&
                    Math.Abs(planar.FaceNormal.DotProduct(normal)) > 0.99 &&
                    planar.Area > bestArea)
                {
                    bestArea = planar.Area;
                    sectionFace = planar;
                }
            }

            if (sectionFace == null) return null;
            var points = GetOuterLoopPoints(sectionFace);
            return points.Count >= 3 ? points : null;
        }

        /// <summary>Lát cắt: hình chữ nhật lớn hơn bao hình vế thang, dày vài mm, nằm trên mặt phẳng cắt.</summary>
        private static Solid? CreateSectionSlab(Element stair, Plane plane)
        {
            var bounds = stair.get_BoundingBox(null);
            if (bounds == null) return null;

            var normal = plane.Normal;
            var u = normal.CrossProduct(XYZ.BasisZ);
            if (u.GetLength() < 1e-6)
            {
                u = normal.CrossProduct(XYZ.BasisX);
            }

            if (u.GetLength() < 1e-6) return null;
            u = u.Normalize();
            var v = normal.CrossProduct(u).Normalize();

            var corners = new List<XYZ>();
            foreach (var x in new[] { bounds.Min.X, bounds.Max.X })
            foreach (var y in new[] { bounds.Min.Y, bounds.Max.Y })
            foreach (var z in new[] { bounds.Min.Z, bounds.Max.Z })
            {
                corners.Add(new XYZ(x, y, z));
            }

            const double margin = 1.0;
            var minU = corners.Min(point => point.DotProduct(u)) - margin;
            var maxU = corners.Max(point => point.DotProduct(u)) + margin;
            var minV = corners.Min(point => point.DotProduct(v)) - margin;
            var maxV = corners.Max(point => point.DotProduct(v)) + margin;

            XYZ Corner(double du, double dv) => plane.Origin + u * du + v * dv;

            var loop = new CurveLoop();
            loop.Append(Line.CreateBound(Corner(minU, minV), Corner(maxU, minV)));
            loop.Append(Line.CreateBound(Corner(maxU, minV), Corner(maxU, maxV)));
            loop.Append(Line.CreateBound(Corner(maxU, maxV), Corner(minU, maxV)));
            loop.Append(Line.CreateBound(Corner(minU, maxV), Corner(minU, minV)));

            return GeometryCreationUtilities.CreateExtrusionGeometry(
                new List<CurveLoop> { loop },
                normal,
                SlabThicknessMm.MmToFeet());
        }

        private static Solid? GetStairSolid(Element stair)
        {
            var geometry = stair.get_Geometry(new Options
            {
                DetailLevel = ViewDetailLevel.Fine,
                ComputeReferences = false,
                IncludeNonVisibleObjects = false
            });
            if (geometry == null) return null;

            Solid? best = null;
            foreach (var item in geometry)
            {
                switch (item)
                {
                    case Solid solid when solid.Faces.Size > 0 && solid.Volume > 0:
                        if (best == null || solid.Volume > best.Volume) best = solid;
                        break;
                    case GeometryInstance instance:
                        var instanceGeometry = instance.GetInstanceGeometry();
                        if (instanceGeometry == null) break;
                        foreach (var nested in instanceGeometry)
                        {
                            if (nested is Solid nestedSolid && nestedSolid.Faces.Size > 0 && nestedSolid.Volume > 0 &&
                                (best == null || nestedSolid.Volume > best.Volume))
                            {
                                best = nestedSolid;
                            }
                        }

                        break;
                }
            }

            return best;
        }

        /// <summary>Phương án dự phòng khi phép cắt solid thất bại: mặt đứng lớn nhất của vế thang.</summary>
        private static List<XYZ>? GetFallbackOutline(Element stair)
        {
            var geometry = stair.get_Geometry(new Options
            {
                DetailLevel = ViewDetailLevel.Fine,
                ComputeReferences = false,
                IncludeNonVisibleObjects = false
            });
            if (geometry == null) return null;

            var faces = new List<PlanarFace>();
            CollectPlanarFaces(geometry, faces);

            var face = faces
                .Where(candidate => Math.Abs(candidate.FaceNormal.Z) < 0.15 && candidate.Area > 1e-4)
                .OrderByDescending(candidate => candidate.Area)
                .FirstOrDefault();
            if (face == null) return null;

            var points = GetOuterLoopPoints(face);
            return points.Count >= 3 ? points : null;
        }

        private static void CollectPlanarFaces(GeometryElement? geometry, List<PlanarFace> faces)
        {
            if (geometry == null) return;

            foreach (var item in geometry)
            {
                switch (item)
                {
                    case Solid solid when solid.Faces.Size > 0:
                        foreach (Face face in solid.Faces)
                        {
                            if (face is PlanarFace planar && planar.Area > 0)
                            {
                                faces.Add(planar);
                            }
                        }

                        break;
                    case GeometryInstance instance:
                        CollectPlanarFaces(instance.GetInstanceGeometry(), faces);
                        break;
                }
            }
        }

        private static List<XYZ> GetOuterLoopPoints(PlanarFace face)
        {
            var loop = face.GetEdgesAsCurveLoops()
                .OrderByDescending(curveLoop => curveLoop.GetExactLength())
                .FirstOrDefault();
            if (loop == null) return [];

            var points = new List<XYZ>();
            foreach (var curve in loop)
            {
                foreach (var point in curve.Tessellate())
                {
                    if (points.Count == 0 || points[^1].DistanceTo(point) > 1e-6)
                    {
                        points.Add(point);
                    }
                }
            }

            if (points.Count > 1 && points[0].DistanceTo(points[^1]) < 1e-6)
            {
                points.RemoveAt(points.Count - 1);
            }

            return points;
        }

        private static XYZ GetOutlineNormal(IReadOnlyList<XYZ> outline)
        {
            for (var index = 1; index < outline.Count - 1; index++)
            {
                var normal = (outline[index] - outline[0]).CrossProduct(outline[index + 1] - outline[0]);
                if (normal.GetLength() > 1e-6)
                {
                    return normal.Normalize();
                }
            }

            return XYZ.BasisY;
        }
    }
}
