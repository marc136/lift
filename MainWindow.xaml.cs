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

            AddDataToListBox();
        }
        
        private void AddDataToListBox()
        {
            lbShownCommands.ItemsSource = _data.Executables;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_data.Executables);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Category");
            view.GroupDescriptions.Add(groupDescription);

            _data.Executables.CollectionChanged += Executables_CollectionChanged;
        }

        void Executables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // save to task bar
            var thisJumpList = JumpList.GetJumpList(Application.Current);

            thisJumpList.ShowFrequentCategory = false;
            thisJumpList.ShowRecentCategory = false;
            thisJumpList.JumpItems.Clear();

            /**/
            foreach (var exe in _data.Executables)
            {
                var task = exe.ToJumpTask();
                thisJumpList.JumpItems.Add(task);
            }/**/

            thisJumpList.Apply();
            //thisJumpList.SetJumpList(Application.Current, jumpList);
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

        private void btnAddACommand_Click(object sender, RoutedEventArgs e)
        {
            CreateOrEditCommand();
            //_data.Executables.Add(new OneExe(@"d:\Hello.exe", category: "Another Category"));
        }

        private void lbShownCommands_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var uiElement = sender as System.Windows.Controls.Primitives.Selector;
            
            if (lbShownCommands.SelectedIndex == -1) return;
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;

            CreateOrEditCommand(item);
        }

        private void CreateOrEditCommand(OneExe item = null)
        {
            bool editMode = true;
            if (item == null)
            {
                editMode = false;
                item = new OneExe();
            }

            var editWindow = new EditEntry();
            editWindow.UseItem(item);

            editWindow.ShowDialog();
            if (editWindow.Success && editWindow.Entry != null)
            {
                if (editMode) _data.Executables.Remove(item);
                _data.Executables.Add(editWindow.Entry);
            }
        }

        private void lbShownCommands_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // add delete command
        }
    }

}
