// --------------------------------------------------------------------------------------------------------------------
// Filename : Application.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Model
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    using ModHelper;

    using Utility;

    public class Application : INotifyPropertyChanged
    {
        private ObservableDictionary<ulong, ModLocalItem> _activeMods = new();
        private ObservableDictionary<ulong, ModLocalItem> _localMods = new();
        private string _modFolderPath;

        private bool _modsLoaded;
        private ObservableCollection<string> _profiles = new();

        private string _saveFolderPath;
        private string _selectedProfile;

        public bool ModsLoaded
        {
            get => _modsLoaded;

            set
            {
                _modsLoaded = value;
                OnPropertyChanged();
            }
        }

        public string SaveFolderPath
        {
            get => _saveFolderPath;

            set
            {
                _saveFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string ModFolderPath
        {
            get => _modFolderPath;

            set
            {
                _modFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string SelectedProfile
        {
            get => _selectedProfile;

            set
            {
                _selectedProfile = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Profiles
        {
            get => _profiles;

            set
            {
                _profiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableDictionary<ulong, ModLocalItem> LocalMods
        {
            get => _localMods;

            set
            {
                _localMods = value;
                OnPropertyChanged();
            }
        }

        public ObservableDictionary<ulong, ModLocalItem> ActiveMods
        {
            get => _activeMods;

            set
            {
                _activeMods = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
