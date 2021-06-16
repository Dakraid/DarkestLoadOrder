// --------------------------------------------------------------------------------------------------------------------
// Filename : ModernApplicationViewModel.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 09.06.2021 19:20
// Last Modified On : 09.06.2021 19:21
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.ViewModel
{
    using ModHelper;
    using Ookii.Dialogs.Wpf;
    using Serialization.Savegame;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    using GongSolutions.Wpf.DragDrop;

    using Microsoft.Win32;

    using Newtonsoft.Json;

    using Threading;
    using Utility;
    using Application = Model.Application;
    using DragDrop = System.Windows.DragDrop;

    public class ModernApplicationViewModel : ViewModelBase, IDropTarget
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

        public ModernApplicationViewModel()
        {
            Application = new Application();

            // Outsourced into its own function for async functionality
            ResolveProfiles();

            if (string.IsNullOrWhiteSpace(Application.Settings.ModFolderPath))
                return;

            ModDatabase = new ModDatabase(this);
            ModDatabase.ReadMods();
        }

        private async void ResolveProfiles()
        {
            var profiles = await Tasks.ProfileScanTask.Execute(Application.Settings.SaveFolderPath);

            if (profiles == null || profiles.Count == 0)
            {
                Application.Profiles = new ObservableCollection<string>();
                return;
            }

            Application.Profiles = new ObservableCollection<string>(profiles.Keys);
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Application.WriteConfiguration();
            ModDatabase.WriteDatabase();
        }
        
        public void DragOver(IDropInfo dropInfo)
        {
            // Call default DragOver method, cause most stuff should work by default
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);

            if (dropInfo.Data is not ObservableKeyValuePair<ulong, ModLocalItem> sourceItem || dropInfo.TargetItem is not ObservableKeyValuePair<ulong, ModLocalItem> targetItem)
                return;

            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects           = DragDropEffects.Move;
        }
        
        public void Drop(IDropInfo dropInfo)
        {
            // The default drop handler don't know how to set an item's group. You need to explicitly set the group on the dropped item like this.
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);

            if (dropInfo.VisualTarget is System.Windows.Controls.ListBox listBox)
            {
                listBox.Items.Refresh();
            }

            // Changing group data at runtime isn't handled well: force a refresh on the collection view.
            if (dropInfo.TargetCollection is ICollectionView view)
            {
                view.Refresh();
            }
        }

        private void PSelectProfile(object param)
        {
            var selectedProfile = (string)param;

            if (string.IsNullOrWhiteSpace(selectedProfile))
                return;

            Application.SelectedProfile = selectedProfile;

            var loadProfileTask = new Tasks.LoadProfileTask(Application.Settings.SaveFolderPath, selectedProfile);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();

            while (loadProfileThread.IsAlive)
                Thread.Sleep(500);

            var saveData = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                if (!Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource = ugcs.Value.Source;

                Application.ActiveMods.Add(modId, modLocalItem);
                Application.LocalMods.Remove(modId);
            }

            Application.LoadedProfile = true;
        }
        private RelayCommand _profileSelect; public ICommand SelectProfile
        {
            get
            {
                return _profileSelect ??= new RelayCommand(PSelectProfile,
                           param => true);
            }
        }

        private void PSelectModsFolder()
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select your mods folder.",
                UseDescriptionForTitle = true
            };

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool)dialog.ShowDialog())
                return;

            Application.Settings.ModFolderPath = dialog.SelectedPath;
            ModDatabase.ReadMods();
        }

        private RelayCommand _selectModsCommand; public ICommand SelectModsFolder
        {
            get
            {
                return _selectModsCommand ??= new RelayCommand(param => PSelectModsFolder(),
                           param => true);
            }
        }

        private void PSelectSaveFolder()
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select your save folder.",
                UseDescriptionForTitle = true
            };

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool)dialog.ShowDialog())
                return;

            Application.Settings.SaveFolderPath = dialog.SelectedPath;
        }

        private RelayCommand _selectSaveCommand; public ICommand SelectSaveFolder
        {
            get
            {
                return _selectSaveCommand ??= new RelayCommand(param => PSelectSaveFolder(),
                           param => true);
            }
        }

        private void PSortAvailableMods()
        {
            SortedDictionary<string, ulong> modList = new();

            foreach (var (modId, modLocalItem) in Application.LocalMods)
            {
                modList.Add(modLocalItem.ModTitle, modId);
            }

            var copyMods = Application.LocalMods;
            Application.LocalMods = new ObservableDictionary<ulong, ModLocalItem>();

            foreach (var modId in modList.Values)
            {
                if (!copyMods.TryGetValue(modId, out var mod))
                    continue;

                Application.LocalMods.Add(modId, mod);
            }
        }

        private RelayCommand _sortAvailableMods; public ICommand SortAvailableMods
        {
            get
            {
                return _sortAvailableMods ??= new RelayCommand(param => PSortAvailableMods(),
                           param => true);
            }
        }

        private void PSaveChanges()
        {
            if (!File.Exists(@".\temp\savedata_in.json"))
                return;

            var saveData = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            if (saveData == null)
                return;
            
            saveData.BaseRoot.AppliedUgcs1_0 = new Dictionary<string, AppliedUgcs1_0>();

            var position = 0;

            foreach (var (_, modLocalItem) in Application.ActiveMods)
            {
                var index = position.ToString();

                var mod = new AppliedUgcs1_0
                {
                    Name = modLocalItem.ModPublishedId.ToString(), Source = modLocalItem.ModSource
                };

                saveData.BaseRoot.AppliedUgcs1_0.Add(index, mod);

                position++;
            }

            File.WriteAllText(@".\temp\savedata_out.json", JsonConvert.SerializeObject(saveData));

            var saveProfileTask =
                new Tasks.SaveProfileTask(Application.Settings.SaveFolderPath, Application.SelectedProfile);
            var saveProfileThread = new Thread(saveProfileTask.Execute);
            saveProfileThread.Start();
        }
        private RelayCommand _saveChanges; public ICommand SaveChanges
        {
            get
            {
                return _saveChanges ??= new RelayCommand(param => PSaveChanges(),
                           param => true);
            }
        }

        private void POpenInExplorer(object param)
        {
            var selectedItems = (IList) param;

            if (selectedItems.Count <= 0)
                return;

            foreach (var selectedItem in selectedItems)
            {
                var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                var targetPath = Application.Settings.ModFolderPath + "\\" + modItem.Key;

                Process.Start("explorer.exe", targetPath);
            }
        }

        private RelayCommand _openInExplorer; public ICommand OpenInExplorer
        {
            get
            {
                return _openInExplorer ??= new RelayCommand(POpenInExplorer,
                           param => true);
            }
        }

        private void POpenInSteam(object param)
        {
            var selectedItems = (IList) param;

            if (selectedItems.Count <= 0)
                return;

            var steamExe = (string) Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Valve\\Steam", "SteamExe", null);

            if (string.IsNullOrWhiteSpace(steamExe) || !File.Exists(steamExe))
            {
                MessageBox.Show("Could not locate Steam.exe, please make sure you have it installed.", "Steam.exe not found!");
                return;
            }

            foreach (var selectedItem in selectedItems)
            {
                var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                var targetPath = "steam://url/CommunityFilePage/" + modItem.Key;
                
                Process.Start(steamExe, targetPath);
            }
        }
        private RelayCommand _openInSteam; public ICommand OpenInSteam
        {
            get
            {
                return _openInSteam ??= new RelayCommand(POpenInSteam,
                           param => true);
            }
        }

        private void PResetChanges()
        {
            if (!File.Exists(@".\temp\savedata_in.json"))
                return;

            var saveData = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            if (saveData == null)
                return;

            foreach (var activeMod in Application.ActiveMods)
            {
                Application.ActiveMods.Remove(activeMod);
                Application.LocalMods.Add(activeMod);
            }

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                var item = Application.LocalMods.Keys;

                if (!Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                Application.ActiveMods.Add(modId, modLocalItem);
                Application.LocalMods.Remove(modId);
            }
        }
        private RelayCommand _resetChanges; public ICommand ResetChanges
        {
            get
            {
                return _resetChanges ??= new RelayCommand(param => PResetChanges(),
                           param => true);
            }
        }

        private void PRefreshMods()
        {
            Application.ActiveMods = new ObservableDictionary<ulong, ModLocalItem>();
            Application.LocalMods  = new ObservableDictionary<ulong, ModLocalItem>();

            ModDatabase.ReadDatabase();
            ModDatabase.ReadMods();
            
            if (!File.Exists(@".\temp\savedata_in.json"))
                return;

            var saveData = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            if (saveData == null)
                return;

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                if (!Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                Application.ActiveMods.Add(modId, modLocalItem);
                Application.LocalMods.Remove(modId);
            }
        }
        private RelayCommand _refreshMods; public ICommand RefreshMods
        {
            get
            {
                return _refreshMods ??= new RelayCommand(param => PRefreshMods(),
                           param => true);
            }
        }
    }
}
