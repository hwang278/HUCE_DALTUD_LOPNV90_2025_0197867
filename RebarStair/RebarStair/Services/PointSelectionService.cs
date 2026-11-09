using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RebarStair.RebarStair.Model;
using System;

namespace RebarStair.RebarStair.Services
{
    /// <summary>
    /// Lấy các điểm mốc của vế thang từ người dùng (Revit pick point) và kiểm tra
    /// hình học cơ bản trước khi dựng thép.
    /// </summary>
    public sealed class PointSelectionService
    {
        private readonly UIDocument _uiDocument;

        public PointSelectionService(UIDocument uiDocument) =>
            _uiDocument = uiDocument ?? throw new ArgumentNullException(nameof(uiDocument));

        /// <summary>Chọn bộ điểm tương ứng với loại thang.</summary>
        public void Select(StairModel stairModel, string stairTypeName)
        {
            switch (stairTypeName)
            {
                case "Type1":
                    SelectForType1(stairModel);
                    break;
                case "Type2":
                    SelectForType2(stairModel);
                    break;
                case "Type3":
                    SelectForType3(stairModel);
                    break;
                case "Chiếu Nghỉ":
                case "Landing":
                    SelectForLanding(stairModel);
                    break;
            }
        }

        private void SelectForType1(StairModel stair)
        {
            stair.P1 = _uiDocument.Selection.PickPoint("P1");
            stair.P2 = _uiDocument.Selection.PickPoint("P2");
            stair.P3 = _uiDocument.Selection.PickPoint("P3");
            stair.P4 = _uiDocument.Selection.PickPoint("P4");
            stair.P5 = _uiDocument.Selection.PickPoint("P5");
            stair.P6 = _uiDocument.Selection.PickPoint("P6");
            stair.P7 = _uiDocument.Selection.PickPoint("P7");
            stair.P8 = _uiDocument.Selection.PickPoint("P8");
            stair.P9 = _uiDocument.Selection.PickPoint("P9");
            RequirePerpendicular(stair.P2 - stair.P1, stair.P9 - stair.P1);
        }

        private void SelectForType2(StairModel stair)
        {
            stair.P1 = _uiDocument.Selection.PickPoint("P1");
            stair.P2 = _uiDocument.Selection.PickPoint("P2");
            stair.P3 = _uiDocument.Selection.PickPoint("P3");
            stair.P4 = _uiDocument.Selection.PickPoint("P4");
            stair.P5 = _uiDocument.Selection.PickPoint("P5");
            stair.P6 = _uiDocument.Selection.PickPoint("P6");
            RequirePerpendicular(stair.P2 - stair.P1, stair.P6 - stair.P1);
        }

        private void SelectForType3(StairModel stair)
        {
            stair.P1 = _uiDocument.Selection.PickPoint("P1");
            stair.P2 = _uiDocument.Selection.PickPoint("P2");
            stair.P3 = _uiDocument.Selection.PickPoint("P3");
            stair.P4 = _uiDocument.Selection.PickPoint("P4");
            stair.P5 = _uiDocument.Selection.PickPoint("P5");
            stair.P6 = _uiDocument.Selection.PickPoint("P6");
            stair.P7 = _uiDocument.Selection.PickPoint("P7");
            stair.P8 = _uiDocument.Selection.PickPoint("P8");
            stair.P9 = _uiDocument.Selection.PickPoint("P9");
            stair.P10 = _uiDocument.Selection.PickPoint("P10");
            stair.P11 = _uiDocument.Selection.PickPoint("P11");
            RequirePerpendicular(stair.P2 - stair.P1, stair.P11 - stair.P1);
        }

        private void SelectForLanding(StairModel stair)
        {
            stair.P1 = _uiDocument.Selection.PickPoint();
            stair.P2 = _uiDocument.Selection.PickPoint();
            stair.P3 = _uiDocument.Selection.PickPoint();
            stair.P4 = _uiDocument.Selection.PickPoint();
            RequirePerpendicular(stair.P1 - stair.P3, stair.P3 - stair.P4);
        }

        private static void RequirePerpendicular(XYZ first, XYZ second)
        {
            if (Math.Abs(first.Normalize().DotProduct(second.Normalize())) > 1e-6)
            {
                throw new OperationCanceledException();
            }
        }
    }
}
