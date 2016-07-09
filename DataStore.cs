using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Migo
{
    public class DataStore
    {
        //source: http://www.codeproject.com/Articles/483055/XML-Serialization-and-Deserialization-Part
        private XmlSerializer serializer;

        public bool SortExecutablesNeeded { get; set; }

        private WpfCrutches.ObservableSortedList<OneExe> _executables;
        public WpfCrutches.ObservableSortedList<OneExe> Executables
        {
            get { return _executables ?? (_executables = new WpfCrutches.ObservableSortedList<OneExe>()); }
            private set { _executables = value; }
        }

        public DataStore()
        {
            serializer = new XmlSerializer(Executables.GetType());
        }

        public void Load()
        {
            string loaded = MiSettings.Default.Executables;
            try
            {
                using (var reader = new StringReader(loaded))
                {/** /
                    var templist = serializer.Deserialize(reader) as List<OneExe>;
                    this.Executables = new WpfCrutches.ObservableSortedList<OneExe>(templist);
                 /**/
                    this.Executables = serializer.Deserialize(reader) as WpfCrutches.ObservableSortedList<OneExe>;
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }

            // Add some Default entries if none exist
            if (this.Executables == null || this.Executables.Count == 0)
            {
                InitializeSampleEntries();
            }
        }

        public void Save()
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

        private void InitializeSampleEntries()
        {
            this.Executables.Add(new OneExe() { FilePath = @"C:\bin\Mikogo-host.exe", Category = "My first category" });
            this.Executables.Add(new OneExe() { FilePath = @"C:\bin\Mikogo-host.exe", Arguments = "-s 000000930" });
            this.Executables.Add(new OneExe() { FilePath = @"C:\bin\Mikogo-viewer.exe", Arguments = "-s 000000930" });
            this.Executables.Add(new OneExe() { FilePath = @"C:\bin\SessionPlayer.exe", Category = "Second category" });

            Save();
        }

        internal void RenameCategory(string oldCategoryName, string newCategoryName)
        {
            OneExe[] elems = Executables.ToArray<OneExe>();
            foreach (OneExe elem in elems)
            {
                if (elem.Category == oldCategoryName)
                {
                    elem.Category = newCategoryName;
                }
            }
            Executables = new WpfCrutches.ObservableSortedList<OneExe>(elems);
        }

        private void RenameCategory1(string oldCategoryName, string newCategoryName)
        {
            OneExe[] selected = Executables.Where(e => e.Category.Equals(oldCategoryName)).ToArray();

            foreach (OneExe exe in selected)
            {
                Executables.Remove(exe);
                //exe.SetCategorySilent(newCategoryName);
                exe.Category = newCategoryName;
                Executables.Add(exe);
            }
        }
    }
}
