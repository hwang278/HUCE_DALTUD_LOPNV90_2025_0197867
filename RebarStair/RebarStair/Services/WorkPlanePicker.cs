using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;

namespace RebarStair.RebarStair.Services
{
    /// <summary>
    /// Chọn một mặt phẳng làm work plane cho việc bố trí thép và gán vào view đang mở.
    /// </summary>
    public sealed class WorkPlanePicker
    {
        private readonly UIDocument _uiDocument;
        private readonly Document _document;

        public WorkPlanePicker(UIDocument uiDocument)
        {
            _uiDocument = uiDocument ?? throw new ArgumentNullException(nameof(uiDocument));
            _document = uiDocument.Document;
        }

        /// <summary>Trả về mặt phẳng người dùng đã chọn, null nếu huỷ hoặc chọn mặt không hợp lệ.</summary>
        public PlanarFace? PickAndApply()
        {
            var reference = _uiDocument.Selection.PickObject(ObjectType.Face, "Chọn mặt để tạo WorkPlane");
            var face = _document.GetElement(reference.ElementId)
                .GetGeometryObjectFromReference(reference) as PlanarFace;
            if (face == null)
            {
                TaskDialog.Show("RebarStair", "Chỉ chọn mặt phẳng hợp lệ.");
                return null;
            }

            using var transaction = new Transaction(_document, "Tạo WorkPlane");
            transaction.Start();
            var sketchPlane = SketchPlane.Create(_document, reference);
            _document.ActiveView.SketchPlane = sketchPlane;
            transaction.Commit();

            return face;
        }
    }
}
