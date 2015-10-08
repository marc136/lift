using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Migo
{
    public static class ImageFromExeRetriever
    {
        /**
         * source: http://www.c-sharpcorner.com/uploadfile/dpatra/get-icon-from-filename-in-wpf/
         */
        public static System.Windows.Media.ImageSource RetrieveAsImageSource(string filepath)
        {
            System.Windows.Media.ImageSource image = null;
            if (File.Exists(filepath))
            {
                using (System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filepath))
                {
                    image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        System.Windows.Int32Rect.Empty,
                        //new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height),
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        //System.Windows.Media.Imaging.BitmapSizeOptions.FromHeight(20));
                }
            }

            return image;
        }

        /**
         * source: https://msdn.microsoft.com/de-de/library/ms404308(v=vs.110).aspx
         */
        public static System.Drawing.Icon RetrieveAsIcon(string filepath)
        {
            System.Drawing.Icon icon = null;
            if (File.Exists(filepath))
            {
                icon = System.Drawing.Icon.ExtractAssociatedIcon(filepath);
            }

            return icon;
        }
    }
}
