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
            _data.Executables.CollectionChanged += Executables_CollectionChanged;

            Hagge();

            AddDataToListBox();
        }
        
        private void AddDataToListBox()
        {
            lbShownCommands.ItemsSource = _data.Executables;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_data.Executables);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Category");
            view.GroupDescriptions.Add(groupDescription);
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
            _data.Executables.Add(new OneExe(@"d:\Hello.exe", category: "Another Category"));
        }

        private void lbShownCommands_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var uiElement = sender as System.Windows.Controls.Primitives.Selector;
            
            if (lbShownCommands.SelectedIndex == -1) return;
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;

            item.PropertyChanged += item_PropertyChanged;
            var editWindow = new EditEntry();
            editWindow.UseItem(item);

            editWindow.ShowDialog();
            item.PropertyChanged -= item_PropertyChanged;
            if (!editWindow.Success || editWindow.Entry == null) return;

            var exes = _data.Executables;
            var index = _data.Executables.IndexOf(item);

            if (true)
            {
                _data.Executables.Remove(item);
                _data.Executables.Add(editWindow.Entry);
            }
            else
            {
                _data.Executables[index] = editWindow.Entry;
            }

            Console.WriteLine("double click");
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // sort list again
            // if a category was changed
            // if the title of an item was changed
            switch (e.PropertyName)
            {
                case "Category":
                case "Title":
                    //_data.Executables.OrderBy(i => i.Title);
                    _data.SortExecutablesNeeded = true;
                    break;
            }
        }

        void Executables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Collection was changed.");
            if (_data.SortExecutablesNeeded)
            {
                _data.SortExecutablesNeeded = false;
                //_data.Executables.OrderBy(i => i.Title);
            }
        }


    }

}
