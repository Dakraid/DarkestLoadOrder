// --------------------------------------------------------------------------------------------------------------------
// Filename : ApplicationView.xaml.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 07.06.2021 10:10
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.View
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Microsoft.Win32;

    using ModHelper;

    using Newtonsoft.Json;

    using Ookii.Dialogs.Wpf;

    using Serialization.Savegame;

    using Threading;

    using Utility;

    using ViewModel;

    /// <summary>
    ///     Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        public ApplicationView()
        {
            InitializeComponent();
        }

        private bool FilterAvailable(object item)
        {
            var dataContext = (ApplicationViewModel) DataContext;

            return string.IsNullOrEmpty(dataContext.Application.SearchAvailable) || ((ObservableKeyValuePair<ulong, ModLocalItem>) item).Value.ModTitle.Contains(dataContext.Application.SearchAvailable, StringComparison.OrdinalIgnoreCase);
        }

        private void wnd_MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;
            dataContext.CloseView();

            Application.Current.Shutdown();
        }

        private void btn_SelectSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select your save folder.", UseDescriptionForTitle = true
            };

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog())
                return;

            var dataContext = (ApplicationViewModel) DataContext;
            dataContext.Application.SaveFolderPath = dialog.SelectedPath;
        }

        private void btn_SelectModFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select your workshop folder.", UseDescriptionForTitle = true
            };

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog())
                return;

            var dataContext = (ApplicationViewModel) DataContext;
            dataContext.Application.ModFolderPath = dialog.SelectedPath;
        }

        private void cbx_ProfileSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;
            var newSelect   = e.AddedItems[0]?.ToString();

            if (string.IsNullOrWhiteSpace(newSelect))
                return;

            dataContext.Application.SelectedProfile = newSelect;

            var loadProfileTask   = new Tasks.LoadProfileTask(dataContext.Application.SaveFolderPath, newSelect);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();

            while (loadProfileThread.IsAlive)
                Thread.Sleep(500);

            var saveData = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                if (!dataContext.Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                dataContext.Application.ActiveMods.Add(modId, modLocalItem);
                dataContext.Application.LocalMods.Remove(modId);
            }

            dataContext.Application.LoadedProfile = true;

            var viewAvailable = (CollectionView) CollectionViewSource.GetDefaultView(lbx_AvailableMods.ItemsSource);

            viewAvailable.Filter = FilterAvailable;
        }

        private void btn_Undo_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;
            var saveData    = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            foreach (var activeMod in dataContext.Application.ActiveMods)
            {
                dataContext.Application.ActiveMods.Remove(activeMod);
                dataContext.Application.LocalMods.Add(activeMod);
            }

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                var item = dataContext.Application.LocalMods.Keys;

                if (!dataContext.Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                dataContext.Application.ActiveMods.Add(modId, modLocalItem);
                dataContext.Application.LocalMods.Remove(modId);
            }
        }

        private void btn_SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;
            var saveData    = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));
            saveData.BaseRoot.AppliedUgcs1_0 = new Dictionary<string, AppliedUgcs1_0>();

            var position = 0;

            foreach (var item in lbx_LoadOrder.Items)
            {
                var modItem = (ObservableKeyValuePair<ulong, ModLocalItem>) item;

                var index = position.ToString();

                var mod = new AppliedUgcs1_0
                {
                    Name = modItem.Value.ModPublishedId.ToString(), Source = modItem.Value.ModSource
                };

                saveData.BaseRoot.AppliedUgcs1_0.Add(index, mod);

                position++;
            }

            File.WriteAllText(@".\temp\savedata_out.json", JsonConvert.SerializeObject(saveData));

            var saveProfileTask =
                new Tasks.SaveProfileTask(dataContext.Application.SaveFolderPath, dataContext.Application.SelectedProfile);
            var saveProfileThread = new Thread(saveProfileTask.Execute);
            saveProfileThread.Start();
        }

        private void btn_SortAvail_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;

            SortedDictionary<string, ulong> modList = new();

            foreach (var item in lbx_AvailableMods.Items)
            {
                var modItem = (ObservableKeyValuePair<ulong, ModLocalItem>) item;
                modList.Add(modItem.Value.ModTitle, modItem.Key);
            }

            var copyMods = dataContext.Application.LocalMods;
            dataContext.Application.LocalMods = new ObservableDictionary<ulong, ModLocalItem>();

            foreach (var modId in modList.Values)
            {
                if (!copyMods.TryGetValue(modId, out var mod))
                    continue;

                dataContext.Application.LocalMods.Add(modId, mod);
            }
        }

        private void click_OpenInExplorer(object sender, RoutedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;

            if (lbx_AvailableMods.SelectedIndex != -1)
            {
                foreach (var selectedItem in lbx_AvailableMods.SelectedItems)
                {
                    var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                    var targetPath = dataContext.Application.ModFolderPath + "\\" + modItem.Key;

                    Process.Start("explorer.exe", targetPath);
                }
                lbx_AvailableMods.SelectedIndex = -1;
            }
            else if (lbx_LoadOrder.SelectedIndex != -1)
            {
                foreach (var selectedItem in lbx_LoadOrder.SelectedItems)
                {
                    var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                    var targetPath = dataContext.Application.ModFolderPath + "\\" + modItem.Key;

                    Process.Start("explorer.exe", targetPath);
                }
                lbx_LoadOrder.SelectedIndex = -1;
            }
        }

        private void click_OpenInSteam(object sender, RoutedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;
            var steamExe    = (string) Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Valve\\Steam", "SteamExe", null);
            
            if (string.IsNullOrWhiteSpace(steamExe) || !File.Exists(steamExe))
            {
                MessageBox.Show("Could not locate Steam.exe, please make sure you have it installed.", "Steam.exe not found!");
                return;
            }

            if (lbx_AvailableMods.SelectedIndex != -1)
            {
                foreach (var selectedItem in lbx_AvailableMods.SelectedItems)
                {
                    var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                    var targetPath = "steam://url/CommunityFilePage/" + modItem.Key;

                    Process.Start(steamExe, targetPath);
                }
                lbx_AvailableMods.SelectedIndex = -1;
            }
            else if (lbx_LoadOrder.SelectedIndex != -1)
            {
                foreach (var selectedItem in lbx_LoadOrder.SelectedItems)
                {
                    var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                    var targetPath = "steam://url/CommunityFilePage/" + modItem.Key;

                    Process.Start(steamExe, targetPath);
                }
                lbx_LoadOrder.SelectedIndex = -1;
            }
        }

        private async void txb_AvailableSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;

            if (string.IsNullOrEmpty(txb_AvailableSearch.Text)) dataContext.Application.SearchAvailable = null;

            async Task<bool> UserKeepsTyping()
            {
                var txt = txb_AvailableSearch.Text;
                await Task.Delay(250);

                return txt != txb_AvailableSearch.Text;
            }

            if (await UserKeepsTyping() || txb_AvailableSearch.Text == dataContext.Application.SearchAvailable) return;
            dataContext.Application.SearchAvailable = txb_AvailableSearch.Text;

            CollectionViewSource.GetDefaultView(lbx_AvailableMods.ItemsSource).Refresh();
        }

        private async void txb_ActiveSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;

            if (string.IsNullOrEmpty(txb_ActiveSearch.Text)) dataContext.Application.SearchActive = null;

            async Task<bool> UserKeepsTyping()
            {
                var txt = txb_ActiveSearch.Text;
                await Task.Delay(250);

                return txt != txb_ActiveSearch.Text;
            }

            if (await UserKeepsTyping() || txb_ActiveSearch.Text == dataContext.Application.SearchAvailable) return;
            dataContext.Application.SearchActive = txb_ActiveSearch.Text;

            if (string.IsNullOrWhiteSpace(dataContext.Application.SearchActive))
            {
                lbx_LoadOrder.SelectedItems.Clear();
                return;
            }

            foreach (var item in lbx_LoadOrder.Items)
            {
                var modItem  = (ObservableKeyValuePair<ulong, ModLocalItem>) item;

                if (!modItem.Value.ModTitle.Contains(dataContext.Application.SearchActive, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                lbx_LoadOrder.SelectedItems.Add(item);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (ApplicationViewModel) DataContext;

            dataContext.Application.ActiveMods = new ObservableDictionary<ulong, ModLocalItem>();
            dataContext.Application.LocalMods  = new ObservableDictionary<ulong, ModLocalItem>();

            dataContext.ModDatabase.ReadDatabase(dataContext.Application.ModFolderPath);
            dataContext.ModDatabase.ResolveMods(dataContext);

            var saveData = SaveData.FromJson(File.ReadAllText(@".\temp\savedata_in.json"));

            foreach (var ugcs in saveData.BaseRoot.AppliedUgcs1_0)
            {
                if (!ulong.TryParse(ugcs.Value.Name, out var modId))
                    continue;

                if (!dataContext.Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                dataContext.Application.ActiveMods.Add(modId, modLocalItem);
                dataContext.Application.LocalMods.Remove(modId);
            }
        }
    }
}
