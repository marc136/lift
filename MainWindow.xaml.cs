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
            
            AddDataToListBox();

            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _data.Save();
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
            foreach (var exe in _data.Executables)
            {
                var task = exe.ToJumpTask();
                thisJumpList.JumpItems.Add(task);
            }/**/

            thisJumpList.Apply();
            if (newJumpList) JumpList.SetJumpList(Application.Current, thisJumpList);
        }

        public void ClearJumpList()
        {
            var jumpList = JumpList.GetJumpList(Application.Current);
            if (jumpList != null)
            {
                jumpList.JumpItems.Clear();
                jumpList.Apply();
            }
        }

        private void btnAddACommand_Click(object sender, RoutedEventArgs e)
        {
            CreateOrEditCommand();
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

        #region Context Menu clicks
        private void SingleEntryEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            CreateOrEditCommand(item);
        }

        private void SingleEntryDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            _data.Executables.Remove(item);
        }
        #endregion

    }

}
