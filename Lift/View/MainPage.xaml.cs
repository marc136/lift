using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace Lift.View
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {

        public Data.LiftItems LiftItems { get; private set; }
        protected Data.Options Options { get; private set; }

        private CollectionView itemCollectionView;

        private Data.LiftItem selectedItem;
        private DragDropHelper dragHelper;

        public MainPage()
        {
            InitializeComponent();
            SetWindowTitle();
            dragHelper = new DragDropHelper();

            Options = Persistence.OptionsStore.Load();
            LiftItems = Persistence.LiftItemsStore.Load();
            DataContext = LiftItems;

            // move this block to XAML, see http://www.galasoft.ch/mydotnet/articles/article-2007081301.aspx
            itemCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(LiftItems);
            itemCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            itemCollectionView.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Descending));
            itemCollectionView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            LiftItems.CollectionChanged += (sender, e) =>
            {
                Console.WriteLine(DateTime.Now + " LiftItems.CollectionChanged/2 triggered");
                //UpdateLiftItems();
            };
            lbLiftItems.SelectedIndex = 0;
            lbLiftItems.Focus();
        }

        private void SetWindowTitle()
        {
            Application.Current.MainWindow.Title = "Lift starter";
        }

        private void btnOptions_Click(object btnSender, RoutedEventArgs btnEvent)
        {
            var optionPage = new View.Options(Options, LiftItems);
            optionPage.Return += (sender, e) =>
            {
                SetWindowTitle(); // update the window title after returning to this page
                Options = e.Result;
                Persistence.OptionsStore.Save(Options);
            };

            this.NavigationService.Navigate(optionPage);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedItem();
        }

        private void EditSelectedItem()
        {
            selectedItem = lbLiftItems.SelectedItem as Data.LiftItem;
            if (selectedItem != null) EditItem(selectedItem);
        }

        private void EditItem(Data.LiftItem item)
        {
            var edit = new View.EditItem(item);
            edit.Return += (sender, e) =>
            {
                SetWindowTitle(); // update the window title after returning to this page
                Data.LiftItem changed = e.Result;
                if (changed == null) return;
                if (selectedItem != null) LiftItems.Remove(selectedItem);
                LiftItems.Add(changed);
                UpdateLiftItems();
            };

            this.NavigationService.Navigate(edit);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            selectedItem = null;
            EditItem(null);
        }

        /**
         * A user may drop files onto 
         *   1. An item -> category of that item is used
         *   2. A category header -> category is used
         *   3. The ListBox/ScrollViewer -> no category is set (-> default is used)
         */
        #region Drop Events Handler
        private void lbLiftItems_DropOn(object sender, DragEventArgs e)
        {
            // will be added to the 'none' category
            DropEvent_OnCategory("", e);
        }

        private void GroupHeader_DropOn(object sender, DragEventArgs e)
        {
            var context = (sender as GroupItem)?.DataContext as CollectionViewGroup;
            if (context == null) return;

            e.Handled = true;
            string category = context.Name as string;
            DropEvent_OnCategory(category, e);
        }

        private void SingleItem_DropOn(object sender, DragEventArgs e)
        {
            var item = (sender as ListBoxItem)?.Content as Data.LiftItem;
            if (item == null) return;

            e.Handled = true;
            string category = item.Category;
            DropEvent_OnCategory(category, e);
        }

        private void DropEvent_OnCategory(string category, DragEventArgs e)
        {
            var src = e.OriginalSource;
            //string[] dataFormats = e.Data.GetFormats(false);
            //string[] dataFormatsWithConvertibleTo = e.Data.GetFormats(true);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string path in files)
                {
                    LiftItems.Add(new Data.LiftItem() { Category = category, FilePath = path });
                }
                UpdateLiftItems();
            }
            else if (e.Data.GetDataPresent("itemDragged"))
            {
                var dropped = e.Data.GetData("itemDragged") as Data.LiftItem;
                // @todo no remove and add, but update instead
                // If the item would become the first in the list, an error is thrown (out of range) if the category is changed directly
                LiftItems.Remove(dropped);
                dropped.Category = category;
                LiftItems.Add(dropped);
                UpdateLiftItems();
            }
        }
        #endregion

        private void lbLiftItems_MouseMove(object sender, MouseEventArgs e)
        {
            // If at least one list element is being dragged
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

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            Key pressedKey = e.Key;
            Console.WriteLine("Pressed key: '{0}'", e.Key);
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Right:
                    // start application
                    var item = lbLiftItems.SelectedItem as Data.LiftItem;
                    StartProcess(item);
                    break;
                case Key.Space: // space does not work
                case Key.Left:
                case Key.E:
                case Key.F2:
                    // edit item
                    EditSelectedItem();
                    break;
                case Key.Subtract:
                case Key.Delete:
                case Key.Back: // backspace
                case Key.OemMinus:
                    // remove item
                    DeleteSelectedItem();
                    break;
                case Key.Insert:
                    // add new item
                    EditItem(null);
                    break;
                default:
                    Console.WriteLine("Pressed key: '{0}'", e.Key);
                    break;
            }
        }

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
                var category = (item.Content as CollectionViewGroup)?.Name as string;
                if (category != null)
                {
                    //this is triggered even if only clicking on list item
                    e.Handled = true;
                    PromptNewCategoryName(category);
                }
            }
        }
        #endregion

        private void StartProcess(Data.LiftItem item)
        {
            Helpers.StartLiftItem.StartProcess(item);
        }

        private void UpdateLiftItems()
        {
            itemCollectionView.Refresh();
            Persistence.LiftItemsStore.Save(LiftItems);

            Helpers.JumpListHelper.Update(itemCollectionView.Cast<Data.LiftItem>());
            //Helpers.JumpListHelper.Update(LiftItems);
        }

        private void DeleteSelectedItem()
        {
            var item = lbLiftItems.SelectedItem as Data.LiftItem;
            if (item == null) return;

            if (Options.PromptOnDelete)
            {
                var text = "Do you really want to delete '" + item.Title + "'?";
                var result = MessageBox.Show(text, "Delete menuItem", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result != MessageBoxResult.Yes) return;
            }

            LiftItems.Remove(item);
            UpdateLiftItems();
        }

        private void PromptNewCategoryName(string category)
        {
            string newCategoryName = InputDialog.Prompt("Edit category name", "Edit", category);

            if (!String.IsNullOrWhiteSpace(newCategoryName))
            {
                LiftItems.RenameCategory(category, newCategoryName);

                UpdateLiftItems();
            }
        }

        private void ListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem == null) return;

            if (e.ClickCount == 1)
            { // dragging a LiftItem might start
                dragHelper.StartPosition = e.GetPosition(null);
                dragHelper.ListBoxItem = listBoxItem;
                dragHelper.Entry = dragHelper.ListBoxItem.Content as Data.LiftItem;
            }
            else
            {
                var liftItem = listBoxItem.Content as Data.LiftItem;
                if (liftItem != null)
                {
                    e.Handled = true;
                    selectedItem = liftItem;
                    EditSelectedItem();
                }
            }
        }

        #region Context menu for a single ListBoxItem
        private void ListBoxItem_ContextMenu_Edit_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedItem();
        }

        private void ListBoxItem_ContextMenu_Duplicate_Click(object sender, RoutedEventArgs e)
        {
            LiftItems.AddClone(lbLiftItems.SelectedItem as Data.LiftItem);
            UpdateLiftItems();
        }

        private void ListBoxItem_ContextMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItem();
        }
        #endregion
    }
}
