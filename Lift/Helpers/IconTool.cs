using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Lift
{
    public static class IconTool
    {
        /**
         * source: http://www.c-sharpcorner.com/uploadfile/dpatra/get-icon-from-filename-in-wpf/
         */
        public static System.Windows.Media.ImageSource RetrieveIconForExeAsImageSource(string filepath)
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
        public static System.Drawing.Icon RetrieveIconForExeAsIcon(string filepath)
        {
            System.Drawing.Icon icon = null;
            if (File.Exists(filepath))
            {
                icon = System.Drawing.Icon.ExtractAssociatedIcon(filepath);
            }

            return icon;
        }

        /**
         * source: http://www.brad-smith.info/blog/archives/164
         */
        public static ExeIconInfo GetAssociatedExeForExtension(string extension)
        {
            RegistryKey keyForExt = Registry.ClassesRoot.OpenSubKey(extension);
            if (keyForExt == null) return null;

            string className = Convert.ToString(keyForExt.GetValue(null));
            RegistryKey keyForClass = Registry.ClassesRoot.OpenSubKey(className);
            if (keyForClass == null) return null;

            RegistryKey keyForIcon = keyForClass.OpenSubKey("DefaultIcon");
            if (keyForIcon == null)
            {
                RegistryKey keyForCLSID = keyForClass.OpenSubKey("CLSID");
                if (keyForCLSID == null) return null;

                string clsid = "CLSID\\"
                    + Convert.ToString(keyForCLSID.GetValue(null))
                    + "\\DefaultIcon";
                keyForIcon = Registry.ClassesRoot.OpenSubKey(clsid);
                if (keyForIcon == null) return null;
            }

            string[] defaultIcon = Convert.ToString(keyForIcon.GetValue(null)).Split(',');
            int index = (defaultIcon.Length > 1) ? Int32.Parse(defaultIcon[1]) : 0;

            var result = new ExeIconInfo() { FilePath = defaultIcon[0], IconIndex = index };
            return result;
        }
    }

    public class ExeIconInfo
    {
        public string FilePath { get; set; }
        public int IconIndex { get; set; }
    }
}
