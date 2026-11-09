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
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // User canceled picking in Revit.
            }
            catch (OperationCanceledException)
            {
                // User canceled or tolerance check was not met.
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Lỗi", ex.ToString());
            }
            finally
            {
                ViewModel?.CurrentWindow?.Show();
            }
        }
        public string GetName() => "Run Rebar";
    }



}