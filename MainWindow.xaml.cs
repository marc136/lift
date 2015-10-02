using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace Migo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataStore _data;

        public MainWindow()
        {
            InitializeComponent();
            _data = new DataStore();
            _data.Load();

            Hagge();
        }
        


        public void Hagge()
        {
            //path to wordpad
            var wpad = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "write.exe");

            JumpTask entry = makeOne();

            JumpList jumpList = new JumpList();

            jumpList.JumpItems.Add(entry);

            entry = makeOne();
            entry.CustomCategory = "Horst";
            jumpList.JumpItems.Add(entry);

            entry = makeOne();
            entry.CustomCategory = "_edit";
            entry.Title = "Edit entries";
            jumpList.JumpItems.Add(entry);

            entry = makeOne();
            entry.CustomCategory = null;
            entry.Title = "Moin";
            jumpList.JumpItems.Add(entry);

            /*
            foreach (var exe in executables)
            {
                var task = exe.ToJumpTask();
                task.CustomCategory = "Starter Category";
                jumpList.JumpItems.Add(task);
            }/**/

            jumpList.ShowFrequentCategory = false;
            jumpList.ShowRecentCategory = false;

            JumpList.SetJumpList(Application.Current, jumpList);
        }

        private JumpTask makeOne()
        {
            return new JumpTask
            {
                Title = "Check for Updates",
                Arguments = "/update",
                Description = "Checks for Software Updates",
                CustomCategory = "Left",
                IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            };
        }

        public void ClearJumpList()
        {
            var jumpList = JumpList.GetJumpList(Application.Current);
            jumpList.JumpItems.Clear();
            jumpList.Apply();
        }

    }
}
