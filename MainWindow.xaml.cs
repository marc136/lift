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
        DragDropHelper dragHelper;
        MigoJumplist jumplistHelper;

        public MainWindow()
        {
            InitializeComponent();
            _data = new DataStore();
            _data.Load();

            jumplistHelper = new MigoJumplist();

            AddDataToListBox();
            dragHelper = new DragDropHelper();
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
            jumplistHelper.Update(_data.Executables);
        }

        private void btnAddACommand_Click(object sender, RoutedEventArgs e)
        {
            CreateOrEditCommand();
        }

        private void lbShownCommands_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var uiElement = sender as System.Windows.Controls.Primitives.Selector;
            /* // not used
            if (lbShownCommands.SelectedIndex == -1) return;
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;

            CreateOrEditCommand(item);
            /**/
        }

        private void SingleEntry_DoubleClick(object sender, RoutedEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item != null)
            {
                var exe = item.Content as OneExe;
                if (exe != null)
                {
                    e.Handled = true;
                    this.CreateOrEditCommand(exe);
                }
            }
        }

        /// <summary>
        /// Will display the editWindow to either create a new Executable entry or edit an existing one
        /// </summary>
        /// <param name="item">Optional entry that should be edited</param>
        private void CreateOrEditCommand(OneExe item = null)
        {
            bool editMode = (item != null) ? true : false;

            var editWindow = new EditEntry();
            editWindow.UseItem(item);

            editWindow.ShowDialog();
            if (editWindow.Success && editWindow.Entry != null)
            {
                if (editMode) _data.Executables.Remove(item);
                _data.Executables.Add(editWindow.Entry);
            }
        }

        #region Single entry click events
        private void SingleEntry_ContextMenu_Edit_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            CreateOrEditCommand(item);
        }

        private void SingleEntry_ContextMenu_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            var item = lbShownCommands.SelectedItem as OneExe;
            if (item == null) return;
            var clone = OneExe.Clone(item);
            _data.Executables.Add(clone);
        }

        private void SingleEntry_ContextMenu_Delete_Click(object sender, RoutedEventArgs e)
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


        private void SingleEntry_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var a = sender;
            if (e.ClickCount == 1)
            {// Cannot use DoubleClick event because it is triggered after a double click on a ListBoxItem (and e.Handled does not work)
                //sender as GroupItem
                dragHelper.StartPosition = e.GetPosition(null);
                dragHelper.ListBoxItem = sender as ListBoxItem;
                dragHelper.Entry = dragHelper.ListBoxItem.Content as OneExe;
            }
        }
        #endregion

        #region Group header element click events
        private void GroupHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {// Cannot use DoubleClick event because it is triggered after a double click on a ListBoxItem (and e.Handled does not work)
                GroupHeader_RenameGroupItem(sender as GroupItem, e);
            }
        }

        private void GroupHeader_ContextMenu_Rename_Click(object sender, RoutedEventArgs e)
        {
            // retrieve the category name
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var menu = menuItem.CommandParameter as ContextMenu;
                if (menu != null)
                {
                    GroupHeader_RenameGroupItem(menu.PlacementTarget as GroupItem, e);
                }
            }
        }

        /// <summary>
        /// retrieves the category name and calls PromptNewCategoryName(category)
        /// </summary>
        /// <param name="item">GroupItem of the ListBox</param>
        private void GroupHeader_RenameGroupItem(GroupItem item, RoutedEventArgs e)
        {
            if (item != null && !e.Handled)
            {
                var group = item.Content as CollectionViewGroup;
                if (group != null)
                {
                    var category = group.Name as string;
                    if (category != null)
                    {
                        //this is triggered even if only clicking on list item
                        e.Handled = true;
                        PromptNewCategoryName(category);
                    }

                }
            }
        }
        #endregion

        private void PromptNewCategoryName(string category)
        {
            string newCategoryName = InputDialog.Prompt("Edit category name", "Edit", category);

            if (!String.IsNullOrWhiteSpace(newCategoryName))
            {
                _data.RenameCategory(category, newCategoryName);

                lbShownCommandsView.Refresh();
                UpdateJumpList();
            }
        }

        #region Drag Element
        private void lbShowCommands_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && dragHelper.Entry != null)
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = dragHelper.StartPosition - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DataObject dragData = new DataObject("itemDragged", dragHelper.Entry);
                    DragDrop.DoDragDrop(dragHelper.ListBoxItem, dragData, DragDropEffects.Move);
                }
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

        private void GroupHeader_DropOn(object sender, DragEventArgs e)
        {
            var group = sender as GroupItem;
            if (group == null) return;
            var context = group.DataContext as CollectionViewGroup;
            if (context == null) return;

            e.Handled = true;
            string category = context.Name as string;
            DropEvent_OnCategory(category, e);
        }

        private void SingleEntry_DropOn(object sender, DragEventArgs e)
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
                foreach (string path in files)
                {
                    exe = new OneExe() { Category = category, FilePath = path };
                    _data.Executables.Add(exe);
                }
            }
            else if (e.Data.GetDataPresent("itemDragged"))
            {
                OneExe dropped = e.Data.GetData("itemDragged") as OneExe;
                // If the item would become the first in the list, an error is thrown (out of range) if the category is changed directly
                _data.Executables.Remove(dropped);
                dropped.Category = category;
                _data.Executables.Add(dropped);
            }
        }
        #endregion
    }

}
