using System.Windows.Shell;

namespace Migo
{
    class MigoJumplist
    {
        public MigoJumplist() { }

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
                var task = CreateJumpTaskItem(exe);
                thisJumpList.JumpItems.Add(task);
            }/**/

            thisJumpList.Apply();
            if (newJumpList) JumpList.SetJumpList(System.Windows.Application.Current, thisJumpList);
        }

        private JumpTask CreateJumpTaskItem(OneExe entry)
        {
            var result = new JumpTask
            {
                Title = entry.Title,
                Arguments = entry.Arguments,
                Description = entry.Hint,
                CustomCategory = entry.Category,
                IconResourcePath = entry.FilePath,
                ApplicationPath = entry.FilePath
            };

            var ext = System.IO.Path.GetExtension(entry.FilePath);
            if (ext != ".exe")
            {
                var info = IconTool.GetAssociatedExeForExtension(ext);
                if (info != null)
                {
                    result.IconResourcePath = info.FilePath;
                    result.IconResourceIndex = info.IconIndex;
                }
            }

            return result;
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
