using Autodesk.Revit.UI;
using RebarStair.RebarStair.ViewModel;

namespace RebarStair.RebarStair.ExternalEvents
{
    public class RunRebarHandler : IExternalEventHandler
    {
        public RebarStairViewModel ViewModel { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                ViewModel?.Run();
                ViewModel?.CurrentWindow?.Show();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Lỗi", ex.ToString());
            }
        }
        public string GetName() => "Run Rebar";
    }



}