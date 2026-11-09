using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Rebar.RebarStair.View;
using RebarStair.Filter;
using RebarStair.helpers;
using RebarStair.RebarStair.Services;
using RebarStair.RebarStair.ViewModel;


namespace RebarStair.RebarStair

{
    
    [Transaction(TransactionMode.Manual)]
    public class RebarStairCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;
            try
            {
                var reference = uiDoc.Selection.PickObject(
                    ObjectType.Element,
                    new StairSelectionFilter(),
                    "Chọn cầu thang cần bố trí thép");
                var stair = doc.GetElement(reference);
                if (stair == null) return Result.Cancelled;
                var logPath = GeometryInspector.Inspect(doc, stair);
                TaskDialog.Show(
                    "RebarStair Geometry Inspector",
                    $"Đã ghi thông tin model được chọn vào:{Environment.NewLine}{logPath}");

                using var group = new TransactionGroup(doc, "Create stair rebar");
                group.Start();
                using var preview = RevitPreviewSession.Create(doc, stair);
                var services = new RebarStairServices(doc, uiDoc, new PreviewService(preview));
                var vm = new RebarStairViewModel(services, stair);
                while (true)
                {
                    var view = new RebarStairView();
                    vm.AttachWindow(view);
                    view.DataContext = vm;
                    view.ShowDialog();

                    if (vm.IsPreviewRequested)
                    {
                        vm.CreateRebarPreview();
                        continue;
                    }

                    if (vm.IsWorkPlaneRequested)
                    {
                        vm.PickWorkPlane();
                        continue;
                    }

                    break;
                }

                vm.ClearViewport();

                if (vm.IsAccepted && vm.HasPreview)
                {
                    preview.DeleteView();
                    group.Assimilate();
                    GeometryInspector.LogEvent("External command", "Rebar changes committed.");
                    return Result.Succeeded;
                }

                group.RollBack();
                GeometryInspector.LogEvent("External command", "Rebar changes rolled back.");
                return Result.Cancelled;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception exception)
            {
                GeometryInspector.LogException("External command", exception);
                message = exception.ToString();
                return Result.Failed;
            }
        }
    }
}
