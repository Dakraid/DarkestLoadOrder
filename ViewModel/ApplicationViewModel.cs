using System.Collections.ObjectModel;
using System.Linq;
using DarkestLoadOrder.Model;
using DarkestLoadOrder.Utility;

namespace DarkestLoadOrder.ViewModel
{
    internal class ApplicationViewModel
    {
        private Config _config;
        private Application _application;

        public ApplicationViewModel()
        {
            _config = new Config();
            _application = new Application();
            _application.SaveFolderPath = _config.Properties.SaveFolderPath;
            _application.ModFolderPath = _config.Properties.ModFolderPath;
            _application.Profiles = new ObservableCollection<string>(_config.ScanProfiles().Keys);
        }
    }
}