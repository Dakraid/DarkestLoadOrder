using System.Collections.ObjectModel;
using System.ComponentModel;
using DarkestLoadOrder.ModHelper;
using DarkestLoadOrder.Utility;

namespace DarkestLoadOrder.Model
{
    public class Application : INotifyPropertyChanged
    {
        public ObservableDictionary<ulong, ModLocalItem> ActiveMods;
        public ObservableDictionary<ulong, ModLocalItem> LocalMods;
        public ObservableCollection<string> Profiles;
        
        private string _saveFolderPath;
        private string _modFolderPath;
        private string _selectedProfile;

        public string SaveFolderPath
        {
            get => _saveFolderPath;
            set
            {
                _saveFolderPath = value;
                OnPropertyChanged("SaveFolderPath");
            }
        }

        public string ModFolderPath
        {
            get => _modFolderPath;
            set
            {
                _modFolderPath = value;
                OnPropertyChanged("ModFolderPath");
            }
        }

        public string SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                _selectedProfile = value;
                OnPropertyChanged("SelectedProfile");
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}