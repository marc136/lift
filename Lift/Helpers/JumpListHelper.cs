using System.Collections.Generic;
using System.Windows.Shell;

namespace Lift.Helpers
{
    class JumpListHelper
    {
        public static void Update(IEnumerable<Data.LiftItem> entries)
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
            
            foreach (var entry in entries)
            {
                var task = CreateJumpTaskItem(entry);
                thisJumpList.JumpItems.Add(task);
            }

            thisJumpList.Apply();
            if (newJumpList) JumpList.SetJumpList(System.Windows.Application.Current, thisJumpList);
        }

        private static JumpTask CreateJumpTaskItem(Data.LiftItem entry)
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

        public static void ClearJumpList()
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
