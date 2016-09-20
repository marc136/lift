using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lift
{
    public class DataStore
    {
        //source: http://www.codeproject.com/Articles/483055/XML-Serialization-and-Deserialization-Part
        private XmlSerializer serializer;

        public bool SortExecutablesNeeded { get; set; }

        private WpfCrutches.ObservableSortedList<LiftItem> _executables;
        public WpfCrutches.ObservableSortedList<LiftItem> Executables
        {
            get { return _executables ?? (_executables = new WpfCrutches.ObservableSortedList<LiftItem>()); }
            private set { _executables = value; }
        }

        public DataStore()
        {
            serializer = new XmlSerializer(Executables.GetType());
        }

        public void LoadFromSettings()
        {
            string loaded = MiSettings.Default.Executables;
            try
            {
                using (var reader = new StringReader(loaded))
                {/** /
                    var templist = serializer.Deserialize(reader) as List<LiftItem>;
                    this.Executables = new WpfCrutches.ObservableSortedList<LiftItem>(templist);
                 /**/
                    Executables = serializer.Deserialize(reader) as WpfCrutches.ObservableSortedList<LiftItem>;
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }

            // Add some Default entries if none exist
            if (Executables == null || Executables.Count == 0)
            {
                InitializeSampleEntries();
            }
        }

        public void ImportFromFile(string filepath)
        {
            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    Executables = serializer.Deserialize(reader) as WpfCrutches.ObservableSortedList<LiftItem>;
                    SaveToSettings();
                }
            }
            catch (InvalidOperationException ex)
            {
                // do something with this exception, report it back to the user
                Console.WriteLine("Could not import the file '{0}', Exception: {1}", filepath, ex);
            }
            catch (Exception ex)
            {
                // an error that occurred while opening the file
                Console.WriteLine("Could read the file '{0}', Exception: {1}", filepath, ex);
            }
        }

        public void SaveToSettings()
        {
            string str;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, Executables);
                str = writer.ToString();
            }

            MiSettings.Default.Executables = str;
            MiSettings.Default.Save();
        }

        public void ExportToFile(string filepath)
        {
            try
            {
                using (var file = new System.IO.StreamWriter(filepath))
                {
                    serializer.Serialize(file, Executables);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing the file, do something about it! {0}", ex.ToString());
            }
        }

        private void InitializeSampleEntries()
        {
            this.Executables.Add(new LiftItem() { FilePath = @"C:\bin\Mikogo-host.exe", Category = "My first category" });
            this.Executables.Add(new LiftItem() { FilePath = @"C:\bin\Mikogo-host.exe", Arguments = "-s 000000930" });
            this.Executables.Add(new LiftItem() { FilePath = @"C:\bin\Mikogo-viewer.exe", Arguments = "-s 000000930" });
            this.Executables.Add(new LiftItem() { FilePath = @"C:\bin\SessionPlayer.exe", Category = "Second category" });

            SaveToSettings();
        }

        internal void RenameCategory(string oldCategoryName, string newCategoryName)
        {
            LiftItem[] elems = Executables.ToArray<LiftItem>();
            foreach (LiftItem elem in elems)
            {
                if (elem.Category == oldCategoryName)
                {
                    elem.Category = newCategoryName;
                }
            }
            Executables = new WpfCrutches.ObservableSortedList<LiftItem>(elems);
        }

        private void RenameCategory1(string oldCategoryName, string newCategoryName)
        {
            LiftItem[] selected = Executables.Where(e => e.Category.Equals(oldCategoryName)).ToArray();

            foreach (LiftItem exe in selected)
            {
                Executables.Remove(exe);
                //exe.SetCategorySilent(newCategoryName);
                exe.Category = newCategoryName;
                Executables.Add(exe);
            }
        }
    }
}
