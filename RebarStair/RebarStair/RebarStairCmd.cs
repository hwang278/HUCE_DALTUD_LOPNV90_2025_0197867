using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Rebar.RebarStair.View;
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
            var uiApp = commandData.Application;
            var vm = new RebarStairViewModel(uiApp, doc, uiDoc);
            var view = new RebarStairView
            {
                DataContext = vm
            };
            vm.RebarStairView = view;
            vm.CurrentWindow = view;
            vm.ShowWindowAgain = () =>
            {
                view.Dispatcher.Invoke(() =>
                {
                    view.Show();
                    view.Activate();
                });
            };
            view.ShowDialog();

            return Result.Succeeded;
        }
    }
}
