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

        public Options()
        {
            PromptOnDelete = true;
        }
    }
}
