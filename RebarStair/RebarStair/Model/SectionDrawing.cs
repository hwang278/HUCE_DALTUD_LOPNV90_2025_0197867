using System;
using System.Collections.Generic;

namespace RebarStair.RebarStair.Model
{
    /// <summary>Điểm 2D trên canvas mặt cắt (đơn vị pixel của canvas).</summary>
    public readonly record struct Point2D(double X, double Y);

    /// <summary>
    /// Dữ liệu mặt cắt 2D đã sẵn sàng để vẽ: biên dạng bê tông, các đường thép và kích thước canvas.
    /// Không phụ thuộc WPF — view tự chuyển sang hình học để vẽ.
    /// </summary>
    public sealed class SectionDrawing
    {
        public SectionDrawing(
            IReadOnlyList<Point2D> outline,
            IReadOnlyList<IReadOnlyList<Point2D>> rebarPolylines,
            double width,
            double height,
            string scaleText)
        {
            Outline = outline ?? throw new ArgumentNullException(nameof(outline));
            RebarPolylines = rebarPolylines ?? throw new ArgumentNullException(nameof(rebarPolylines));
            Width = width;
            Height = height;
            ScaleText = scaleText;
        }

        public IReadOnlyList<Point2D> Outline { get; }

        public IReadOnlyList<IReadOnlyList<Point2D>> RebarPolylines { get; }

        public double Width { get; }

        public double Height { get; }

        public string ScaleText { get; }
    }
}
