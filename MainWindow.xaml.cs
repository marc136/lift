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
        CollectionView lbShownCommandsView;

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

            lbShownCommandsView = (CollectionView)CollectionViewSource.GetDefaultView(_data.Executables);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Category");
            lbShownCommandsView.GroupDescriptions.Add(groupDescription);
            
            _data.Executables.CollectionChanged += Executables_CollectionChanged;
        }

        void Executables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            lbShownCommandsView.Refresh();
            UpdateJumpList();
        }

        private void UpdateJumpList() 
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

        #region Context Menu on item clicks
        private void SingleEntryEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            CreateOrEditCommand(item);
        }

        private void SingleEntryDuplicate_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            var clone = new OneExe();
            clone.UpdateWith(item);
            _data.Executables.Add(clone);
        }

        private void SingleEntryDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            var text = "Do you really want to delete '" + item.Title + "'?";
            var result = MessageBox.Show(text, "Delete menuItem", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                _data.Executables.Remove(item);
            }
        }
        #endregion

        #region Context Menu on header clicks
        private void GroupHeaderRename_Click(object sender, RoutedEventArgs e)
        {
            // retrieve the category name
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var menu = menuItem.CommandParameter as ContextMenu;
            if (menu == null) return;
            var groupItem = menu.PlacementTarget as GroupItem;
            if (groupItem == null) return;
            var group = groupItem.Content as CollectionViewGroup;
            if (group == null) return;
            var category = group.Name as string;

            if (category != null) PromptNewCategoryName(category);
        }

        private void PromptNewCategoryName(string category)
        {
            string newCategoryName = InputDialog.Prompt("Edit category name", "Edit", category);

            if (newCategoryName != null)
            {
                foreach (OneExe exe in _data.Executables)
                {
                    if (exe.Category.Equals(category))
                    {
                        exe.SetCategorySilent(newCategoryName);
                    }
                }

                lbShownCommandsView.Refresh();
                UpdateJumpList();
            }
        }

        #endregion

        /**
         * A user may drop files onto 
         *   1. An item -> category of that item is used
         *   2. A category header -> category is used
         *   3. The ListBox/ScrollViewer -> no category is set (-> default is used)
         */
        #region Drop Events Handler
        private void lbShownCommands_Drop(object sender, DragEventArgs e)
        {
            DropEvent_OnCategory("", e);
        }

        private void lbShownCommands_DropOn_GroupItem(object sender, DragEventArgs e)
        {
            var group = sender as GroupItem;
            if (group == null) return;
            var context = group.DataContext as CollectionViewGroup;
            if (context == null) return;

            e.Handled = true;
            string category = context.Name as string;
            DropEvent_OnCategory(category, e);
        }

        private void lbShownCommands_DropOn_ListBoxItem(object sender, DragEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item == null) return;
            var exe = item.Content as OneExe;
            if (exe == null) return;
            
            e.Handled = true;
            string category = exe.Category;
            DropEvent_OnCategory(category, e);
        }

        private void DropEvent_OnCategory(string category, DragEventArgs e)
        {
            var src = e.OriginalSource;

            var data = e.Data;
            string[] dataFormats = e.Data.GetFormats(false);
            string[] dataFormatsWithConvertibleTo = e.Data.GetFormats(true);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                OneExe exe;
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string path in files) {
                    exe = new OneExe() { Category = category, FilePath = path };
                    _data.Executables.Add(exe);
                }
            }
        }
        #endregion
    }

}
