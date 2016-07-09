using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace Migo
{
    class MigoJumplist
    {
        public MigoJumplist()
        {

        }

        public void Update(WpfCrutches.ObservableSortedList<OneExe> entries)
        {
            // save to task bar
            var thisJumpList = JumpList.GetJumpList(System.Windows.Application.Current);
            bool newJumpList = false;

            if (thisJumpList == null)
            {
                newJumpList = true;
                thisJumpList = new JumpList();
            }

            thisJumpList.ShowFrequentCategory = false;
            thisJumpList.ShowRecentCategory = false;
            thisJumpList.JumpItems.Clear();

            /**/
            foreach (var exe in entries)
            {
                var task = exe.ToJumpTask();
                thisJumpList.JumpItems.Add(task);
            }/**/

            thisJumpList.Apply();
            if (newJumpList) JumpList.SetJumpList(System.Windows.Application.Current, thisJumpList);
        }

        public void ClearJumpList()
        {
            var jumpList = JumpList.GetJumpList(System.Windows.Application.Current);
            if (jumpList != null)
            {
                jumpList.JumpItems.Clear();
                jumpList.Apply();
            }
        }
    }
}
