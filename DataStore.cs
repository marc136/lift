using System;
using System.Collections.Generic;
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

        private List<OneExe> _executables;
        public List<OneExe> Executables
        {
            get { return _executables ?? (_executables = new List<OneExe>()); }
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
                {
                    this.Executables = serializer.Deserialize(reader) as List<OneExe>;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("InvalidOperationException while deserializing settings.");
            }
            
            // Add some Default entries if none exist
            if (this.Executables.Count == 0)
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
            this.Executables.Add(new OneExe(@"C:\bin\Mikogo-host.exe"));
            this.Executables.Add(new OneExe(@"C:\bin\Mikogo-host.exe", arguments: "-s 000000930"));

            Save();
        }
    }
}
