namespace RebarStair.RebarStair.Model
{
    /// <summary>
    /// Toàn bộ tham số hình học người dùng nhập trên giao diện, đơn vị mm.
    /// Là dữ liệu thuần, không phụ thuộc Revit hay WPF.
    /// </summary>
    public sealed class RebarParameters
    {
        public double Cover { get; init; } = 20;

        public double Spacing { get; init; } = 150;

        public double StairWidth { get; init; } = 1000;

        public double LandingWidth { get; init; } = 1000;

        public double F1 { get; init; } = 300;

        public double F2 { get; init; } = 300;

        public double L1 { get; init; } = 150;

        public double L2 { get; init; } = 300;

        public double BeamWidth { get; init; } = 200;

        public double BeamHeight { get; init; } = 300;

        public double AnchorageLength { get; init; } = 150;

        public double ExtensionLength { get; init; } = 150;
    }
}
