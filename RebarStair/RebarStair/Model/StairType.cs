using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RebarStair.RebarStair.Model
{
   public class StairType
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }

        public ImageSource ImageSource => LoadImage(ImagePath);
        public ImageSource ImageSource1 => LoadImage(ImagePath1);
        public ImageSource ImageSource2 => LoadImage(ImagePath2);
        public ImageSource ImageSource3 => LoadImage(ImagePath3);
        private ImageSource LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                return null;
            }
        }
    }
}
