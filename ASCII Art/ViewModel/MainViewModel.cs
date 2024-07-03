using System.ComponentModel;
namespace ASCII_Art.ViewModel
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        public string fileName;
        public string fileSize;
        public string outputDir;
        public string infoText;
        public bool canStart;
        public string startButtonName;

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public string FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }

        public string OutputDir
        {
            get { return outputDir; }
            set
            {
                outputDir = value;
                OnPropertyChanged("OutputDir");
            }
        }

        public string InfoText
        {
            get { return infoText; }
            set
            {
                infoText = value;
                OnPropertyChanged("InfoText");
            }
        }

        public bool CanStart
        {
            get { return canStart; }
            set
            {
                canStart = value;
                OnPropertyChanged("CanStart");
            }
        }

        public string StartButtonName
        {
            get { return startButtonName; }
            set
            {
                startButtonName = value;
                OnPropertyChanged("StartButtonName");
            }
        }
    }
}
