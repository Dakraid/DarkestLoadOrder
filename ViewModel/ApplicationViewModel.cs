namespace DarkestLoadOrder.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using GongSolutions.Wpf.DragDrop;

    using ModHelper;

    using Utility;

    using Application = Model.Application;

    internal class ApplicationViewModel : ViewModelBase, IDropTarget
    {
        private bool _loadedProfile;

        private Application _application;
        private Config      _config;
        private ModDatabase _modDatabase;

        public ApplicationViewModel()
        {
            Config = new Config();

            Application = new Application
            {
                SaveFolderPath = Config.Properties.SaveFolderPath, ModFolderPath = Config.Properties.ModFolderPath, Profiles = new ObservableCollection<string>(Config.ScanProfiles().Keys)
            };

            ModDatabase = new ModDatabase();

            if (string.IsNullOrWhiteSpace(_application.ModFolderPath))
                return;
            
            ModDatabase.ReadDatabase(Application.ModFolderPath);
            ResolveMods();
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

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);

            var sourceItem = dropInfo.Data as ObservableKeyValuePair<ulong, ModLocalItem>;
            var targetItem = dropInfo.TargetItem as ObservableKeyValuePair<ulong, ModLocalItem>;

            if (sourceItem == null || targetItem == null)
                return;
            
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects           = DragDropEffects.Move;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            
            // The default drop handler don't know how to set an item's group. You need to explicitly set the group on the dropped item like this.
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);

            // Now extract the dragged group items and set the new group (target)
            var data = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<ObservableKeyValuePair<ulong, ModLocalItem>>().ToList();
            foreach (var groupedItem in data)
            {
                // groupedItem.Group = dropInfo.TargetGroup.Name.ToString();
            }

            // Changing group data at runtime isn't handled well: force a refresh on the collection view.
            if (dropInfo.TargetCollection is ICollectionView view)
            {
                view.Refresh();
            }
        }

        private async void ResolveMods()
        {
            HashSet<ulong> knownMods = null;

            if (_modDatabase.KnownMods?.Count > 0)
            {
                knownMods = new HashSet<ulong>();

                foreach (var (modId, modDatabaseItem) in _modDatabase.KnownMods)
                {
                    _application.LocalMods.Add(modId, modDatabaseItem);
                    knownMods.Add(modId);
                }
            }

            Dictionary<ulong, ModLocalItem> newMods;

            if (knownMods?.Count > 0)
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
                if (!_modDatabase.KnownMods.ContainsKey(modId))
                    _modDatabase.KnownMods.Add(modId, modLocalItem);

            _modDatabase.WriteDatabase();
        }
    }
}
