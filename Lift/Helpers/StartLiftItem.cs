using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lift.Data;

namespace Lift.Helpers
{
    class StartLiftItem
    {
        internal static void StartProcess(LiftItem item)
        {
            if (item == null || String.IsNullOrWhiteSpace(item.FilePath)) return;

            if (String.IsNullOrWhiteSpace(item.Arguments))
            {
                // no arguments launch file with default application
                System.Diagnostics.Process.Start(item.FilePath);
            }
            else
            {
                // Use ProcessStartInfo class.
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = item.FilePath;
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = item.Arguments;

                System.Diagnostics.Process.Start(startInfo);
            }
        }
    }
}
