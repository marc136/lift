using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lift.Persistence
{
    internal class OptionsStore
    {
        internal static Data.Options Load()
        {
            return LoadFromSettings();
        }

        private static Data.Options LoadFromSettings()
        {
            Data.Options result = null;
            var serializer = new XmlSerializer(typeof(Data.Options));
            string loaded = Persistence.MiSettings.Default.Options;

            try
            {
                using (var reader = new StringReader(loaded))
                {
                    result = serializer.Deserialize(reader) as Data.Options;
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }

            if (result == null) result = new Data.Options();
            return result;
        }

        internal static void Save(Data.Options options)
        {
            SaveToSettings(options);
        }

        internal static void SaveToSettings(Data.Options options)
        {
            string str;
            var serializer = new XmlSerializer(typeof(Data.Options));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, options);
                str = writer.ToString();
            }

            Persistence.MiSettings.Default.Options = str;
            Persistence.MiSettings.Default.Save();
        }
    }
}
