using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lift.Persistence
{
    internal class LiftItemsStore
    {
        private static XmlSerializer NewLiftItemsSerializer()
        {
            return new XmlSerializer(typeof(Data.LiftItems));
        }

        internal static Data.LiftItems Load()
        {
            var result = LoadLiftItemsFromSettings();
            // Add some Default entries if none exist
            if (result == null || result.Count == 0)
            {
                result = GenerateSampleEntries();
                Save(result);
            }
            return result;
        }

        private static Data.LiftItems LoadLiftItemsFromSettings()
        {
            Data.LiftItems result = null;
            string loaded = Persistence.MiSettings.Default.LiftItems;
            try
            {
                using (var reader = new StringReader(loaded))
                {
                    result = NewLiftItemsSerializer().Deserialize(reader) as Data.LiftItems;
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }

            return result;
        }

        internal static Data.LiftItems ImportFromFile(string filepath)
        {
            Data.LiftItems result = null;
            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    result = NewLiftItemsSerializer().Deserialize(reader) as Data.LiftItems;
                    //SaveToSettings();
                }
            }
            catch (InvalidOperationException ex)
            {
                // do something with this exception, report it back to the user
                var msg = String.Format("Could not import the file '{0}', Exception: {1}", filepath, ex);
                Console.WriteLine(msg);
            }
            catch (Exception ex)
            {
                // an error that occurred while opening the file
                var msg = String.Format("Could read the file '{0}', Exception: {1}", filepath, ex);
                Console.WriteLine(msg);
            }

            return result;
        }

        internal static void Save(Data.LiftItems liftItems)
        {
            SaveToSettings(liftItems);
        }

        private static void SaveToSettings(Data.LiftItems liftItems)
        {
            string str;
            using (var writer = new StringWriter())
            {
                NewLiftItemsSerializer().Serialize(writer, liftItems);
                str = writer.ToString();
            }

            Persistence.MiSettings.Default.LiftItems = str;
            Persistence.MiSettings.Default.Save();
        }

        internal static void ExportToFile(string filepath, Data.LiftItems liftItems)
        {
            try
            {
                using (var file = new System.IO.StreamWriter(filepath))
                {
                    NewLiftItemsSerializer().Serialize(file, liftItems);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing the file, do something about it! {0}", ex.ToString());
            }
        }

        private static Data.LiftItems GenerateSampleEntries()
        {
            var result = new Lift.Data.LiftItems();
            
            var home = new Data.LiftItem()
            {
                FilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Category = "Folders",
                Hint = "My home directory"
            };
            home.Title += " (user folder)";
            result.Add(home);

            var ie = new Data.LiftItem()
            {
                FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Internet Explorer\iexplore.exe"),
                Category = "Programs",
                Title = "Internet Explorer"
            };
            result.Add(ie);

            var hosts = new Data.LiftItem()
            {
                FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts")
            };
            result.Add(hosts);

            return result;
        }
    }
}
