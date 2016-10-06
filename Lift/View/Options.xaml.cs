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
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : PageFunction<Data.Options>
    {
        private Data.LiftItems _liftItems;
        private Data.Options _options;

        public Options(Data.Options options, Data.LiftItems liftItems)
        {
            InitializeComponent();
            _options = options;
            DataContext = options;
            _liftItems = liftItems;
        }

        private void btnImportList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Default export format (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _liftItems.ClearAndImportFromFile(openFileDialog.FileName);
            }
        }

        private void btnExportList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                Persistence.LiftItemsStore.ExportToFile(saveFileDialog.FileName, _liftItems);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var returnObject = new ReturnEventArgs<Data.Options>(_options);
            // Call onreturn method
            OnReturn(returnObject);
        }
    }
}
