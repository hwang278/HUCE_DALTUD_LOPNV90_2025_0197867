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
            
            // Sử dụng đường dẫn embedded resource
            string iconPath = "C:\\Users\\Huyho\\Source\\Repos\\Nhom5-DALTUDTXD-LOPNV90\\RebarStair\\Images\\1172573_imresizer.jpg"; 
            Button4.LargeImage = new BitmapImage(new Uri(iconPath));
            Button4.Image = new BitmapImage(new Uri(iconPath)); 

            PushButton pushButton4 = Panel.AddItem(Button4) as PushButton;
            return Result.Succeeded;
        }
    }
}
