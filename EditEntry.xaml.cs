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
            //this._item = new OneExe(){ FilePath = item.FilePath, Arguments = item.Arguments, Category = item.Category, Title = item.Title };
            this._item = item;
            tbPath.Text = item.FilePath;
            tbArguments.Text = item.Arguments;
            tbCategory.Text = item.Category;
            tbTitle.Text = item.Title;
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
            this.Close();
        }
    }
}
