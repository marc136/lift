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
        private Persistence.GlobalState internalData;

        public Options(Persistence.GlobalState state)
        {
            InitializeComponent();
            internalData = state;

            System.Windows.Application.Current.MainWindow.Title = state.Translations["Options.Title"];
            DataContext = internalData;

            // TODO set itemsSource from xaml (relative path)
            cbLanguage.ItemsSource = internalData.Translations.SupportedLanguages;
            
            SelectActiveLanguage();
            _liftItems = internalData.LiftItems;
        }

        private void SelectActiveLanguage()
        {
            int bestIndex = 0;
            for (int i = cbLanguage.Items.Count-1; i >-1 ; i--)
            {
                var entry = cbLanguage.Items[i] as System.Globalization.CultureInfo;
                if (entry.IetfLanguageTag.Equals(internalData.Options.Locale))
                {
                    bestIndex = i;
                    break;
                }
                else if (bestIndex == 0 && internalData.Options.Locale.StartsWith(entry.TwoLetterISOLanguageName))
                {
                    bestIndex = i;
                }
            }
            if (bestIndex == 0) internalData.Options.Locale = "en";
            cbLanguage.SelectedIndex = bestIndex;
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
            var returnObject = new ReturnEventArgs<Data.Options>(internalData.Options);
            // Call onreturn method
            OnReturn(returnObject);
        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string lang = ((sender as ComboBox)?.SelectedItem as System.Globalization.CultureInfo)?.IetfLanguageTag;
            internalData.Translations.ChangeLocale(lang);
            internalData.Options.Locale = lang;
        }
    }
}
