using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RebarStair.ExternalApplication
{
    public class ExternalApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //Create The Plugin Tab.
            application.CreateRibbonTab("Stair Rebar");

            //Create The Plugin Panel.
            RibbonPanel Panel = application.CreateRibbonPanel("Stair Rebar", "Stair Rebar");
            Assembly assembly = Assembly.GetExecutingAssembly();

            //Create The Plugin Button.
            PushButtonData Button4 = new PushButtonData("btn_Loc1", "Stair Rebar", assembly.Location, "RebarStair.RebarStair.RebarStairCmd");
            PushButton pushButton4 = Panel.AddItem(Button4) as PushButton;

            Uri UriPath4 = new Uri("pack://application:,,,/RebarStair;component/Images/icon.png");
            BitmapImage Image4 = new BitmapImage(UriPath4);
            pushButton4.LargeImage = Image4;
            return Result.Succeeded;
        }
    }
}
