using RebarStair.RebarStair.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rebar.RebarStair.View.Converters
{
    /// <summary>
    /// Chuyển <see cref="SectionDrawing"/> (dữ liệu thuần) thành hình học để vẽ.
    /// ConverterParameter = "outline" (biên dạng bê tông) hoặc "rebar" (đường thép).
    /// </summary>
    public sealed class SectionGeometryConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SectionDrawing drawing) return null;

            var isRebar = string.Equals(parameter as string, "rebar", StringComparison.OrdinalIgnoreCase);
            var geometry = new PathGeometry();

            if (isRebar)
            {
                foreach (var polyline in drawing.RebarPolylines)
                {
                    AddFigure(geometry, polyline, closed: false);
                }
            }
            else
            {
                AddFigure(geometry, drawing.Outline, closed: true);
            }

            if (geometry.Figures.Count == 0) return null;

            geometry.Freeze();
            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();

        private static void AddFigure(PathGeometry geometry, System.Collections.Generic.IReadOnlyList<Point2D> points, bool closed)
        {
            if (points.Count < 2) return;

            var figure = new PathFigure
            {
                StartPoint = new Point(points[0].X, points[0].Y),
                IsClosed = closed,
                IsFilled = closed
            };

            for (var index = 1; index < points.Count; index++)
            {
                figure.Segments.Add(new LineSegment(new Point(points[index].X, points[index].Y), true));
            }

            geometry.Figures.Add(figure);
        }
    }
}
