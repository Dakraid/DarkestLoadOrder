// --------------------------------------------------------------------------------------------------------------------
// Filename : ApplicationViewModel.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using GongSolutions.Wpf.DragDrop;

    using Model;

    using ModHelper;

    using Threading;

    using Utility;

    using Application = Model.Application;
    using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

    public class ApplicationViewModel : ViewModelBase, IDropTarget
    {
        private Application _application;
        private ModDatabase _modDatabase;

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

        public ApplicationViewModel()
        {
            Application = new Application
            {
                SaveFolderPath = Config.Properties.SaveFolderPath, 
                ModFolderPath = Config.Properties.ModFolderPath
            };

            // Outsourced into its own function for async functionality
            ResolveProfiles();

            ModDatabase = new ModDatabase();

            if (string.IsNullOrWhiteSpace(_application.ModFolderPath))
                return;

            ModDatabase.ReadDatabase(Application.ModFolderPath);
            ModDatabase.ResolveMods(this);
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            DragDrop.DefaultDropHandler.DragOver(dropInfo);

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
            DragDrop.DefaultDropHandler.Drop(dropInfo);

            // Now extract the dragged group items and set the new group (target)
            var data = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<ObservableKeyValuePair<ulong, ModLocalItem>>().ToList();

            foreach (var groupedItem in data)
            {
                // groupedItem.Group = dropInfo.TargetGroup.Name.ToString();
            }

            // Changing group data at runtime isn't handled well: force a refresh on the collection view.
            if (dropInfo.TargetCollection is ICollectionView view)
                view.Refresh();
        }

        private async void ResolveProfiles()
        {
            var profiles = await Tasks.ProfileScanTask.Execute(Config.Properties.SaveFolderPath);

            if (profiles == null || profiles.Count == 0)
            {
                Application.Profiles = new ObservableCollection<string>();
                return;
            }
            Application.Profiles = new ObservableCollection<string>(profiles.Keys);
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
