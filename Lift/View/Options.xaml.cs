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
    using ViewModel;
    using Translations = Resources.Localization.Translations;

    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : PageFunction<Data.Options>
    {
        private Data.LiftItems _liftItems;
        private OptionsPage internalData;

        public Options(Data.Options options, Translations translations, Data.LiftItems liftItems)
        {
            InitializeComponent();

            System.Windows.Application.Current.MainWindow.Title = translations["Options.Title"];
            internalData = new OptionsPage { Options = options, Translations = translations };
            DataContext = internalData;

            _liftItems = liftItems;

            foreach (System.Globalization.CultureInfo lang in internalData.Translations.SupportedLanguages)
            {
                cbLanguage.Items.Add(lang.TwoLetterISOLanguageName);
            }
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
            var lang = (sender as ComboBox).SelectedItem as string;
            internalData.Translations.ChangeLocale(lang);
            /*
            string[] languages = new string[] { "en", "de" };
            if (languages.Contains(lang))
            {
                DataContext.Translations
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            }
            Application.Current.MainWindow.Title = Lift.Properties.Resources.Title;
            var res = Lift.Properties.Resources.ResourceManager;
            
            var t = res.GetString("Title");
            Console.WriteLine("getString(title) returned ", t);/**/

        }
    }
}
