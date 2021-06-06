// --------------------------------------------------------------------------------------------------------------------
// Filename : ApplicationView.xaml.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.View
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

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

                var item = dataContext.Application.LocalMods.Keys;

                if (!dataContext.Application.LocalMods.TryGetValue(modId, out var modLocalItem))
                    continue;

                modLocalItem.ModPriority = long.Parse(ugcs.Key);
                modLocalItem.ModSource   = ugcs.Value.Source;

                dataContext.Application.ActiveMods.Add(modId, modLocalItem);
                dataContext.Application.LocalMods.Remove(modId);
            }

            dataContext.LoadedProfile = true;
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

                // shrug
                mod.Source = "Steam";

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
                foreach (var selectedItem in lbx_AvailableMods.SelectedItems)
                {
                    var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                    var targetPath = dataContext.Application.ModFolderPath + "\\" + modItem.Key;

                    Process.Start("explorer.exe", targetPath);
                }
            else if (lbx_LoadOrder.SelectedIndex != -1)
                foreach (var selectedItem in lbx_LoadOrder.SelectedItems)
                {
                    var modItem    = (ObservableKeyValuePair<ulong, ModLocalItem>) selectedItem;
                    var targetPath = dataContext.Application.ModFolderPath + "\\" + modItem.Key;

                    Process.Start("explorer.exe", targetPath);
                }
        }
    }
}
