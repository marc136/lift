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
using System.Windows.Shapes;

namespace Migo
{
    /// <summary>
    /// Interaction logic for EditEntry.xaml
    /// </summary>
    public partial class EditEntry : Window
    {
        private OneExe _item;
        public OneExe Entry { get { return _item; } }
        
        public bool Success { get; private set; }

        public EditEntry()
        {
            InitializeComponent();
            Success = false;
        }

        public void UseItem(OneExe item)
        {
            if (this._item != null) this._item.PropertyChanged -= _item_PropertyChanged;
            
            this._item = new OneExe(){ FilePath = item.FilePath, Arguments = item.Arguments, Category = item.Category, Title = item.Title };
            //this._item = item;

            UpdateEntryFields();
            this._item.PropertyChanged += _item_PropertyChanged;
        }

        private void UpdateEntryFields(string property = "")
        {
            bool all = string.IsNullOrWhiteSpace(property);

            if (all || property.Equals("FilePath"))
                tbPath.Text = _item.FilePath;
            if (all || property.Equals("Arguments"))
                tbArguments.Text = _item.Arguments;
            if (all || property.Equals("Category"))
                tbCategory.Text = _item.Category;
            if (all || property.Equals("Title"))
                tbTitle.Text = _item.Title;
            if (all || property.Equals("Hint"))
                tbTooltip.Text = _item.Hint;
        }

        void _item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateEntryFields(e.PropertyName);
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            _item.FilePath = tbPath.Text;
            _item.Arguments = tbArguments.Text;
            _item.Category = tbCategory.Text;
            _item.Title = tbTitle.Text;
            _item.Hint = tbTooltip.Text;
            this.Close();
        }

        private void btnSelectExe_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            // list of executable files as found here: http://www.howtogeek.com/137270/50-file-extensions-that-are-potentially-dangerous-on-windows/
            openFileDialog.Filter = "Applications (*.group, ...)|*.group;*.bat;*.cmd;*.ps*;*.application;*.gadget;*.com;*.cpl;*.msc;*.jar|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                //tbPath.Text = openFileDialog.FileName;
                _item.FilePath = openFileDialog.FileName;
            }
        }
    }
}
