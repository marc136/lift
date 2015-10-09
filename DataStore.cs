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
            serializer = new XmlSerializer(typeof(List<OneExe>));
        }

        public void Load()
        {
            string loaded = MiSettings.Default.Executables;
            try
            {
                using (var reader = new StringReader(loaded))
                {
                    var templist = serializer.Deserialize(reader) as List<OneExe>;
                    this.Executables = new WpfCrutches.ObservableSortedList<OneExe>(templist);
                }
            }
            catch (InvalidOperationException) {}
            catch (ArgumentNullException) {}
            
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
                var list = Executables.ToList<OneExe>();
                serializer.Serialize(writer, list);
                str = writer.ToString();
            }

            MiSettings.Default.Executables = str;
            MiSettings.Default.Save();
        }
        
        private void InitializeSampleEntries()
        {
            this.Executables.Add(new OneExe(){ FilePath = @"C:\bin\Mikogo-host.exe", Category = "cat 1" });
            this.Executables.Add(new OneExe(){ FilePath = @"C:\bin\Mikogo-host.exe", Arguments = "-s 000000930" });
            this.Executables.Add(new OneExe(){ FilePath = @"C:\bin\Mikogo-viewer.exe", Arguments = "-s 000000930" });
            this.Executables.Add(new OneExe(){ FilePath = @"C:\bin\SessionPlayer.exe", Category ="a01" });

            Save();
        }
    }
}
