using Lift.Data;
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
    public class DataStore
    {
        private static XmlSerializer NewLiftItemsSerializer()
        {
            return new XmlSerializer(typeof(LiftItems));
        }

        public static LiftItems LoadLiftItems()
        {
            var result = LoadLiftItemsFromSettings();
            // Add some Default entries if none exist
            if (result == null || result.Count == 0)
            {
                result = GenerateSampleEntries();
            }
            return result;
        }

        private static LiftItems LoadLiftItemsFromSettings()
        {
            LiftItems result = null;
            string loaded = Persistence.MiSettings.Default.LiftItems;
            try
            {
                using (var reader = new StringReader(loaded))
                {
                    result = NewLiftItemsSerializer().Deserialize(reader) as LiftItems;
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }

            return result;
        }

        public static LiftItems ImportFromFile(string filepath)
        {
            LiftItems result = null;
            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    result = NewLiftItemsSerializer().Deserialize(reader) as LiftItems;
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

        public static void SaveToSettings(LiftItems liftItems)
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

        public static void ExportToFile(string filepath, LiftItems liftItems)
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

        private static LiftItems GenerateSampleEntries()
        {
            var result = new Lift.Data.LiftItems();
            result.Add(new Data.LiftItem() { FilePath = @"C:\bin\Mikogo-host.exe", Category = "My first category" });
            result.Add(new Data.LiftItem() { FilePath = @"C:\bin\Mikogo-host.exe", Arguments = "-s 000000930" });
            result.Add(new Data.LiftItem() { FilePath = @"C:\bin\Mikogo-viewer.exe", Arguments = "-s 000000930" });
            result.Add(new Data.LiftItem() { FilePath = @"C:\bin\SessionPlayer.exe", Category = "Second category" });

            return result;
        }
    }
}
