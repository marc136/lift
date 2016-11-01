using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditItem.xaml
    /// </summary>
    public partial class EditItem : PageFunction<Data.LiftItem>
    {
        private Data.LiftItem Item;
        private Data.LiftItem InitialItem;

        public EditItem(Data.LiftItem item)
        {
            Item = (item != null) ? (Data.LiftItem)item.Clone() : new Data.LiftItem();
            InitialItem = item;
            InitializeComponent();
            btnSave.Focus();
            DataContext = Item;
            Application.Current.MainWindow.Title = "Edit Lift item";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            AbortEditing();
        }

        private void AbortEditing()
        {
            Item = null;
            ReturnItem();
        }

        private void ReturnItem()
        {
            var returnObject = new ReturnEventArgs<Data.LiftItem>(Item);
            // Call onreturn method
            OnReturn(returnObject);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveItem();
        }

        private void SaveItem()
        {
            if (Item.Equals(InitialItem)) AbortEditing();
            else ReturnItem();
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Applications (*.exe, ...)|*.exe;*.group;*.bat;*.cmd;*.ps*;*.application;*.gadget;*.com;*.cpl;*.msc;*.jar|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Item.FilePath = openFileDialog.FileName;
            }
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Item.FilePath = dialog.SelectedPath;
            }
        }

        private void PageFunction_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            switch (key)
            {
                case Key.Escape:
                case Key.Cancel:
                case Key.Back:
                case Key.BrowserBack:
                    AbortEditing();
                    break;
                case Key.Enter:
                    // If the user presses enter inside a text field, the changed property was not yet propagated because the text field did not lose focus (see https://msdn.microsoft.com/en-us/library/system.windows.data.binding.updatesourcetrigger.aspx). Setting focus to the save button triggers the update. Otherwise I would need to update on every keystroke as described on the linked page.
                    btnSave.Focus();
                    SaveItem();
                    break;
            }
        }

        // drop an element -> set file path
        /// <summary>
        /// Triggered for instance if files or folders are dragged from Windows Explorer
        /// </summary>
        /// <returns>true it could handle the drop event</returns>
        private void DropFileOnEditPage(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            Item.FilePath = paths[0];
        }
    }
}
