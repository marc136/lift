using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace Migo
{
    class One
    {
    }

    [Serializable()]
    public class OneExe
    {
        [field: NonSerialized()]
        String _filepath;
        String _title;
        [field: NonSerialized()]
        String _arguments;
        String _description;

        public String Arguments { get; set; }
        public String FilePath { get; set; }

        public OneExe()
        {
            this.FilePath = "none given";
        }

        public OneExe(String filename, string title = "", string description = "", string arguments = "")
        {
            this._filepath = filename;
            this.FilePath = filename;
            this._title = Path.GetFileNameWithoutExtension(this._filepath);
            if (String.IsNullOrEmpty(description))
            {
                this._description = "Start " + Path.GetFileName(this._title);
            }
            else
            {
                this._description = description;
            }
            this.Arguments = arguments;
            this._arguments = arguments;
        }

        public JumpTask ToJumpTask() 
        {
            return new JumpTask
            {
                Title = this._title,
                Arguments = this._arguments,
                Description = this._description,
                CustomCategory = "Start",
                IconResourcePath = this._filepath,
                ApplicationPath = this._filepath
            };
        }
    }
}
