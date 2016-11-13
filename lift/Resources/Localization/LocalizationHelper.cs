using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace Lift.Resources.Localization
{
    using Texts = Dictionary<string, string>;

    public class Translations : TranslationStrings, INotifyPropertyChanged
    {
        private Texts selectedLocale;
        private Texts defaultLocale;

        /// <summary>
        /// Use indexer to access dictionary items
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>getKey(Key)</returns>
        public string this[string Key] { get { return GetKey(Key); } }

        /// <summary>
        /// Enumerate all supported culture infos
        /// </summary>
        public IEnumerable<CultureInfo> SupportedLanguages { get { return AllTexts.Keys; } }

        #region INotifPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Translations()
        {
            defaultLocale = AllTexts[new CultureInfo("en")];

            // select the user's UI language or pick the default language
            selectedLocale = SelectCultureInfo(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Try to match a passed CultureInfo to the ones existing inside the dictionary
        /// </summary>
        /// <returns>Translations in dictionary or default translations</returns>
        private Texts SelectCultureInfo(CultureInfo info)
        {
            Texts result = null;
            if (info == null) return defaultLocale;

            if (AllTexts.TryGetValue(info, out result))
            {// works if language codes match exactly
                return result;
            }

            string lang2letters = info.TwoLetterISOLanguageName;
            foreach (CultureInfo key in AllTexts.Keys)
            {// retrieves e.g. "en" if passed "en-US"
                if (lang2letters.Equals(key.Name))
                    return AllTexts[key];
            }

            foreach (CultureInfo key in AllTexts.Keys)
            {// retrieves e.g. "de-DE" if passed "de-AT"
                if (lang2letters.Equals(key.TwoLetterISOLanguageName))
                    return AllTexts[key];
            }

            return defaultLocale;
        }

        /// <summary>
        /// Retrieve a value from the current dictionary. If the value does not exist, the default language content is displayed. If both don't exist, the string <Key missing> is returned
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKey(string key)
        {
            string result;
            if (!selectedLocale.TryGetValue(key, out result) &&
                selectedLocale != defaultLocale &&
                !defaultLocale.TryGetValue(key, out result))
            {
                result = "<Key '" + key + "' missing>";
            }
            return result;
        }

        public void ChangeLocale(string locale)
        {
            var info = System.Globalization.CultureInfo.CreateSpecificCulture(locale);
            selectedLocale = SelectCultureInfo(new CultureInfo(locale));
            RaisePropertyChanged(string.Empty);

            // also accessible in code-behind
            //System.Windows.Application.Current.MainWindow.Title = GetKey("Title");
        }
    }
}