using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Xml.Serialization;

namespace Migo
{
    [Serializable]
    public class OneExe : INotifyPropertyChanged, IComparable<OneExe>
    {
        #region Properties
        [NonSerialized]
        private System.Windows.Media.ImageSource _image;
        [XmlIgnoreAttribute]
        public System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                if (_image == null && !String.IsNullOrEmpty(FilePath))
                {
                    _image = IconTool.RetrieveIconForExeAsImageSource(FilePath);
                }
                return _image;
            }
            set { _image = value; }
        }

        private string _arguments;
        public string Arguments
        {
            get { return _arguments; }
            set
            {
                if (value != _arguments)
                {
                    _arguments = value;
                    NotifyPropertyChanged("ProgressText");
                }
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return string.IsNullOrWhiteSpace(_filePath) ? "none given" : _filePath; }
            set
            {
                if (value != this._filePath)
                {
                    this._filePath = value;
                    this.Title = "";
                    this.Hint = "";
                    this.ImageSource = null;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }

        public string FileName { get { return Path.GetFileName(FilePath); } }
        public string FolderPath { get { return Path.GetDirectoryName(FilePath); } }

        private string _category;
        public string Category
        {
            get { return String.IsNullOrEmpty(this._category) ? "none" : this._category; }
            set
            {
                if (value != this._category)
                {
                    this._category = value;
                    NotifyPropertyChanged("Category");
                }
            }
        }
        public void SetCategorySilent(string value)
        {
            this._category = value;
        }

        private string _title;
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = (string.IsNullOrWhiteSpace(this._filePath)) ? "" : Path.GetFileName(this._filePath);
                }
                return _title;
            }
            set
            {
                if (value != this._title)
                {
                    this._title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _hint;
        public string Hint
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_hint) && !string.IsNullOrWhiteSpace(_filePath))
                {
                    _hint = "Start " + FileName;
                }
                return _hint;
            }
            set
            {
                if (value != this._hint)
                {
                    this._hint = value;
                    NotifyPropertyChanged("Hint");
                }
            }
        }

        #endregion
        #region Notify Property Changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public OneExe() { }

        public JumpTask ToJumpTask()
        {
            var result = new JumpTask
            {
                Title = this.Title,
                Arguments = this.Arguments,
                Description = this.Hint,
                CustomCategory = this.Category,
                IconResourcePath = this.FilePath,
                ApplicationPath = this.FilePath
            };

            var ext = Path.GetExtension(this.FilePath);
            if (ext != ".exe")
            {
                var info = IconTool.GetAssociatedExeForExtension(ext);
                if (info != null)
                {
                    result.IconResourcePath = info.FilePath;
                    result.IconResourceIndex = info.IconIndex;
                }
            }

            return result;
        }

        public int CompareTo(OneExe other)
        {
            var value = this.Category.CompareTo(other.Category);
            if (value == 0)
            {
                value = this.Title.CompareTo(other.Title);
            }
            return value;
        }

        public static OneExe Clone(OneExe other)
        {
            var clone = new OneExe
            {
                FilePath = other.FilePath,
                Arguments = other.Arguments,
                Category = other.Category,
                Title = other.Title,
                Hint = other.Hint,
                ImageSource = other.ImageSource
                //Icon = other.Icon
            };
            return clone;
        }
    }
}
