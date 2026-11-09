using Autodesk.Revit.DB;
namespace RebarStair.RebarStair.Model
{
    public class StairModel
    {
        private readonly XYZ?[] _points = new XYZ?[12];

        public XYZ P1 { get => GetPoint(0); set => _points[0] = value; }
        public XYZ P2 { get => GetPoint(1); set => _points[1] = value; }
        public XYZ P3 { get => GetPoint(2); set => _points[2] = value; }
        public XYZ P4 { get => GetPoint(3); set => _points[3] = value; }
        public XYZ P5 { get => GetPoint(4); set => _points[4] = value; }
        public XYZ P6 { get => GetPoint(5); set => _points[5] = value; }
        public XYZ P7 { get => GetPoint(6); set => _points[6] = value; }
        public XYZ P8 { get => GetPoint(7); set => _points[7] = value; }
        public XYZ P9 { get => GetPoint(8); set => _points[8] = value; }
        public XYZ P10 { get => GetPoint(9); set => _points[9] = value; }
        public XYZ P11 { get => GetPoint(10); set => _points[10] = value; }
        public XYZ P12 { get => GetPoint(11); set => _points[11] = value; }

        public Element Stair { get; }

        public StairModel(Element stair)
        {
            Stair = stair ?? throw new ArgumentNullException(nameof(stair));
        }

        private XYZ GetPoint(int index) => _points[index]
            ?? throw new InvalidOperationException($"Điểm P{index + 1} chưa được chọn.");
    }
}
