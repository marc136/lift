using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Xml.Serialization;

namespace Lift.Data
{
    [Serializable]
    public class LiftItem : INotifyPropertyChanged, IComparable<LiftItem>, IEquatable<LiftItem>, ICloneable
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
            set { _image = value; NotifyPropertyChanged("ImageSource"); }
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
                    NotifyPropertyChanged(nameof(Arguments));
                }
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return string.IsNullOrWhiteSpace(_filePath) ? "" : _filePath; }
            set
            {
                if (value != this._filePath)
                {
                    _filePath = value;
                    Title = "";
                    Hint = "";
                    ImageSource = null;
                    NotifyPropertyChanged(nameof(FilePath));
                }
            }
        }

        public string FileName => Path.GetFileName(FilePath);
        public string FolderPath => Path.GetDirectoryName(FilePath);

        private string _category;
        public string Category
        {
            get { return String.IsNullOrEmpty(this._category) ? "none" : this._category; }
            set
            {
                if (value != this._category)
                {
                    this._category = value;
                    NotifyPropertyChanged(nameof(Category));
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
                if (string.IsNullOrWhiteSpace(_title))
                {
                    // generate a title from a file path
                    var next = Path.GetFileName(FilePath);
                    if (next != _title)
                    {
                        _title = next;
                        NotifyPropertyChanged(nameof(Title));
                    }
                }
                return _title;
            }
            set
            {
                if (value != this._title)
                {
                    this._title = value;
                    NotifyPropertyChanged(nameof(Title));
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
                    NotifyPropertyChanged(nameof(Hint));
                }
                return _hint;
            }
            set
            {
                if (value != _hint)
                {
                    _hint = value;
                    NotifyPropertyChanged(nameof(Hint));
                }
            }
        }

        #endregion
        #region Notify Property Changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        public LiftItem() {}

        public int CompareTo(LiftItem other)
        {
            var value = this.Category.CompareTo(other.Category);
            if (value == 0)
            {
                value = this.Title.CompareTo(other.Title);
            }
            return value;
        }

        public bool Equals(LiftItem other)
        {
            return FilePath == other?.FilePath && Arguments == other?.Arguments && Category == other?.Category && Title == other?.Title && Hint == other?.Hint;
        }

        public static LiftItem Clone(LiftItem other)
        {
            var clone = new LiftItem
            {
                FilePath = other.FilePath,
                Arguments = other.Arguments,
                Category = other.Category,
                Title = other.Title,
                Hint = other.Hint,
                //ImageSource = other.ImageSource
                //Icon = other.Icon
            };
            return clone;
        }

        public object Clone()
        {
            var clone = new LiftItem
            {
                FilePath = this.FilePath,
                Arguments = this.Arguments,
                Category = this.Category,
                Title = this.Title,
                Hint = this.Hint,
            };
            return clone;
        }

    }
}
