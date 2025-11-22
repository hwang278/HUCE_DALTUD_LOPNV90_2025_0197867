using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;

namespace RebarStair.RebarStair.ExternalEvents
{
    public class PickWorkPlaneHandler : IExternalEventHandler
    {
        public UIDocument UiDoc { get; set; }
        public Action<Plane> CallbackAfterPick { get; set; }

        public void Execute(UIApplication app)
        {
            UIDocument uiDoc = UiDoc ?? app.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                Reference faceRef = uiDoc.Selection.PickObject(ObjectType.Face, "Chọn mặt để tạo WorkPlane");
                if (faceRef == null) return;

                var element = doc.GetElement(faceRef.ElementId);
                var planarFace = element.GetGeometryObjectFromReference(faceRef) as PlanarFace;

                if (planarFace == null)
                {
                    TaskDialog.Show("Lỗi", "Chỉ chọn mặt phẳng hợp lệ.");
                    return;
                }

                Plane plane;

                using (Transaction tx = new Transaction(doc, "Tạo WorkPlane"))
                {
                    tx.Start();
                    SketchPlane sketchPlane = SketchPlane.Create(doc, faceRef);
                    doc.ActiveView.SketchPlane = sketchPlane;
                    plane = sketchPlane.GetPlane();
                    tx.Commit();
                }

                TaskDialog.Show("Work Plane", "WorkPlane đã được tạo");

                void OnIdling(object sender, IdlingEventArgs args)
                {
                    app.Idling -= OnIdling;
                    CallbackAfterPick?.Invoke(plane);
                }

                app.Idling += OnIdling;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                TaskDialog.Show("Thông báo", "Đã hủy chọn workplane.");
                CallbackAfterPick?.Invoke(null);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());
            }
        }


        public string GetName() => "Pick WorkPlane";
    }
}