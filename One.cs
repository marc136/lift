using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace Migo
{
    [Serializable()]
    public class OneExe : INotifyPropertyChanged, IComparable<OneExe>
    {
        #region Properties
        [field: NonSerialized()]
        private System.Drawing.Icon _icon;
        public System.Drawing.Icon Icon
        {
            get
            {
                if (_icon == null && !String.IsNullOrEmpty(FilePath))
                {
                    _icon = ImageFromExeRetriever.RetrieveAsIcon(FilePath);
                }
                return _icon;
            }
            set { _icon = value; }
        }

        [field: NonSerialized()]
        private System.Windows.Media.ImageSource _image;
        public System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                if (_image == null && !String.IsNullOrEmpty(FilePath))
                {
                    _image = ImageFromExeRetriever.RetrieveAsImageSource(FilePath);
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
            get { return _filePath;  }
            set
            {
                if (value != this._filePath)
                {
                    this._filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }
        
        public string FileName { get { return Path.GetFileName(FilePath); } }
        public string FolderPath { get { return Path.GetDirectoryName(FilePath); } }

        private string _category;
        public string Category {
            get { return String.IsNullOrEmpty(this._category) ? "none" : this._category; }
            set {
                if (value != this._category)
                {
                    this._category = value;
                    NotifyPropertyChanged("Category");
                }
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
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
            get { return _hint; }
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

        public OneExe()
        {
            this.FilePath = "none given";
            this.Category = "none";
        }

        public OneExe(String filename, string title = "", string description = "", string arguments = "", string category = "")
        {
            this.FilePath = filename;
            this.Title = Path.GetFileNameWithoutExtension(this.FilePath);
            this.Hint = String.IsNullOrEmpty(description) ? "Start " + Path.GetFileName(this.Title) : description;

            this.Arguments = arguments;
            this.Category = category;
        }

        public JumpTask ToJumpTask() 
        {
            return new JumpTask
            {
                Title = this.Title,
                Arguments = this.Arguments,
                Description = this.Hint,
                CustomCategory = this.Category,
                IconResourcePath = this.FilePath,
                ApplicationPath = this.FilePath
            };
        }



        public int CompareTo(OneExe other)
        {
            var value = this.Category.CompareTo(other.Category);
            if (value == 0) {
                value = this.Title.CompareTo(other.Title);
            }
            return value;
        }
    }
}
