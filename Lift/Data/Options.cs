using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lift.Data
{
    public class Options
    {
        public bool PromptOnDelete { get; set; }
        //public System.Globalization.CultureInfo Locale { get; set; }
        public string Locale { get; set; }

        public Options()
        {
            PromptOnDelete = true;
            Locale = System.Threading.Thread.CurrentThread.CurrentUICulture.IetfLanguageTag;
        }
    }
}
