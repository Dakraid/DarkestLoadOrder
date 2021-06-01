namespace DarkestLoadOrder.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Data;

    using Model;

    using ModHelper;

    using Utility;

    internal class ApplicationViewModel : ViewModelBase
    {
        private ModDatabase _modDatabase;
        private Application _application;
        private Config      _config;

        public ApplicationViewModel()
        {
            Config = new Config();

            Application = new Application
            {
                SaveFolderPath = Config.Properties.SaveFolderPath, ModFolderPath = Config.Properties.ModFolderPath, Profiles = new ObservableCollection<string>(Config.ScanProfiles().Keys)
            };
            
            ModDatabase = new ModDatabase();
            ModDatabase.ReadDatabase();

            if (string.IsNullOrWhiteSpace(_application.ModFolderPath))
                return;
            
            ResolveMods();
        }

        public ModDatabase ModDatabase
        {
            get => _modDatabase;

            set
            {
                if (Equals(value, _modDatabase)) return;
                _modDatabase = value;
                OnPropertyChanged();
            }
        }
        
        public Application Application
        {
            get => _application;

            set
            {
                if (Equals(value, _application)) return;
                _application = value;
                OnPropertyChanged();
            }
        }

        public Config Config
        {
            get => _config;

            set
            {
                if (Equals(value, _config)) return;
                _config = value;
                OnPropertyChanged();
            }
        }

        private async void ResolveMods()
        {
            HashSet<ulong> knownMods = null;

            if (_modDatabase.KnownMods != null && _modDatabase.KnownMods.Count > 0)
            {
                knownMods = new HashSet<ulong>();
                foreach (var (modId, modDatabaseItem) in _modDatabase.KnownMods)
                {
                    _application.LocalMods.Add(modId, modDatabaseItem);
                    knownMods.Add(modId);
                }
            }

            Dictionary<ulong, ModLocalItem> newMods;

            if (knownMods != null && knownMods.Count > 0)
                newMods = await ModResolverOnline.GetModInfos(_application.ModFolderPath, knownMods);
            else
                newMods = await ModResolverOnline.GetModInfos(_application.ModFolderPath);

            if (newMods != null)
                _application.LocalMods.AddRange(newMods);

            _application.ModsLoaded = true;
        }

        public void CloseView()
        {
            _config.Properties.SaveFolderPath = _application.SaveFolderPath;
            _config.Properties.ModFolderPath  = _application.ModFolderPath;
            _config.WriteConfiguration();

            foreach (var (modId, modLocalItem) in _application.LocalMods)
            {
                if (!_modDatabase.KnownMods.ContainsKey(modId))
                    _modDatabase.KnownMods.Add(modId, modLocalItem);
            }
            _modDatabase.WriteDatabase();
        }
    }
}
