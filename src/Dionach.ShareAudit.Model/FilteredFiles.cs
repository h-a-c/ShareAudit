using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;


namespace Dionach.ShareAudit.Model
{ 
    public class FilteredFiles
    {
        private string _files = string.Empty;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Files
        {
            get => _files;
            set
            {
                if (value ==_files)
                {
                    return;
                }

                _files = value;
                OnPropertyChanged();
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


