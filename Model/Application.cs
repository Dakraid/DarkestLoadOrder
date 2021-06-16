// --------------------------------------------------------------------------------------------------------------------
// Filename : Application.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 07.06.2021 16:01
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Model
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Windows;

    using ModHelper;

    using Newtonsoft.Json;

    using Utility;

    public class Store : BaseModel
    {
        private string _saveFolderPath = "";
        private string _modFolderPath = "";


        public string SaveFolderPath 
        {
            get => _saveFolderPath;

            set
            {
                if (Equals(value, _saveFolderPath)) return;
                _saveFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string ModFolderPath
        {
            get => _modFolderPath;

            set
            {
                if (Equals(value, _modFolderPath)) return;
                _modFolderPath = value;
                OnPropertyChanged();
            }
        }
    }

    public class Application : BaseModel
    {
        private const string ConfigPath = @".\DarkestLoadOrder.json";

        private ObservableDictionary<ulong, ModLocalItem> _activeMods = new();
        private ObservableDictionary<ulong, ModLocalItem> _localMods = new();

        private bool _modsLoaded;
        private bool _loadedProfile;
        private ObservableCollection<string> _profiles = new();
        private string _searchActive;
        private string _searchAvailable;
        private string _selectedProfile;
        private Store _settings = new();

        public Application()
        {
            if (!File.Exists(ConfigPath))
            {
                return;
            }

            var tempConfig = JsonConvert.DeserializeObject<Store>(File.ReadAllText(ConfigPath));

            if (tempConfig == null)
            {
                MessageBox.Show("Could not read the configuration file, your settings have been reset.");

                return;
            }

            if (!Directory.Exists(tempConfig.SaveFolderPath) || !Directory.Exists(tempConfig.ModFolderPath))
            {
                MessageBox.Show($"Verification of the directories failed. Your settings have been reset.");

                return;
            }

            Settings = tempConfig;
        }

        public void WriteConfiguration()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Settings), Encoding.UTF8);
        }

        public Store Settings
        {
            get => _settings;

            set
            {
                if (Equals(value, _settings)) return;
                _settings = value;
                OnPropertyChanged();
            }
        }

        public bool ModsLoaded
        {
            get => _modsLoaded;

            set
            {
                if (Equals(value, _modsLoaded)) return;
                _modsLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool LoadedProfile
        {
            get => _loadedProfile;

            set
            {
                if (Equals(value, _loadedProfile)) return;
                _loadedProfile = value;
                OnPropertyChanged();
            }
        }

        public string SearchAvailable
        {
            get => _searchAvailable;

            set
            {
                if (Equals(value, _searchAvailable)) return;
                _searchAvailable = value;
                OnPropertyChanged();
            }
        }

        public string SearchActive
        {
            get => _searchActive;

            set
            {
                if (Equals(value, _searchActive)) return;
                _searchActive = value;
                OnPropertyChanged();
            }
        }

        public string SelectedProfile
        {
            get => _selectedProfile;

            set
            {
                if (Equals(value, _selectedProfile)) return;
                _selectedProfile = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Profiles
        {
            get => _profiles;

            set
            {
                if (Equals(value, _profiles)) return;
                _profiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableDictionary<ulong, ModLocalItem> LocalMods
        {
            get => _localMods;

            set
            {
                if (Equals(value, _localMods)) return;
                _localMods = value;
                OnPropertyChanged();
            }
        }

        public ObservableDictionary<ulong, ModLocalItem> ActiveMods
        {
            get => _activeMods;

            set
            {
                if (Equals(value, _activeMods)) return;
                _activeMods = value;
                OnPropertyChanged();
            }
        }
    }
}
