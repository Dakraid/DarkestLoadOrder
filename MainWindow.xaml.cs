using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DarkestLoadOrder.Json.Savegame;
using DarkestLoadOrder.ModHelper;
using DarkestLoadOrder.Threading;
using DarkestLoadOrder.Utility;
using Ookii.Dialogs.Wpf;

namespace DarkestLoadOrder
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int TypeDelay = 1000;

        private readonly Config _config = new();
        private readonly ModDatabase _modDatabase = new();

        private string _lastProcessedMods;
        private string _lastProcessedSave;

        public Dictionary<ulong, string> _modList;
        public Dictionary<string, string> _profileList;

        public MainWindow()
        {
            InitializeComponent();

            txb_SaveFolder.Text = _config.Properties.SaveFolderPath;
            txb_ModFolder.Text = _config.Properties.ModFolderPath;

            _profileList = _config.ScanProfiles(brd_SaveFolderIndicator);
            _modList = _config.ScanMods(brd_ModFolderIndicator);

            if (_profileList != null && _profileList.Count > 0)
                _config.PopulateProfiles(cbx_ProfileSelect, _profileList.Keys);
        }

        private void btn_SelectSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.", UseDescriptionForTitle = true
            };

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this,
                    "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.",
                    "Sample folder browser dialog");

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog(this))
                return;

            _config.Properties.SaveFolderPath = dialog.SelectedPath;
            _profileList = _config.ScanProfiles(brd_SaveFolderIndicator);
            txb_SaveFolder.Text = _config.Properties.SaveFolderPath;

            if (_profileList != null && _profileList.Count > 0)
                _config.PopulateProfiles(cbx_ProfileSelect, _profileList.Keys);
        }

        private void btn_SelectModFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.", UseDescriptionForTitle = true
            };

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this,
                    "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.",
                    "Sample folder browser dialog");

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog(this))
                return;

            _config.Properties.ModFolderPath = dialog.SelectedPath;
            _modList = _config.ScanMods(brd_ModFolderIndicator);
            txb_ModFolder.Text = _config.Properties.ModFolderPath;
        }

        private void cbx_ProfileSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            brd_ProfileIndicator.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            var newSelect = e.AddedItems[0]?.ToString();

            if (string.IsNullOrWhiteSpace(newSelect))
                return;

            var loadProfileTask = new Tasks.LoadProfileTask(_modDatabase, _config.Properties.SaveFolderPath, newSelect);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();

            while (loadProfileThread.IsAlive)
                Thread.Sleep(500);

            var saveData = SaveData.FromJson(File.ReadAllText(@".\savedata_out.json"));

            Dictionary<ulong, ModLocalItem> LoadOrder = new();

            foreach (var modUGC in saveData.BaseRoot.AppliedUgcs1_0)
            {
                var modId = ulong.Parse(modUGC.Value.Name);
                _modDatabase.KnownMods.TryGetValue(modId, out var modItem);

                var localMod = new ModLocalItem(modItem)
                {
                    ModEnabled = true,
                    ModPriority = ulong.Parse(modUGC.Key),
                    ModSource = modUGC.Value.Source
                };
                LoadOrder.Add(modId, localMod);
            }

            foreach (var (modId, modItem) in _modDatabase.KnownMods)
            {
                var localMod = new ModLocalItem(modItem)
                {
                    ModEnabled = false,
                    ModPriority = 999,
                    ModSource = "Steam"
                };

                if (!LoadOrder.ContainsKey(modId))
                    LoadOrder.Add(modId, localMod);
            }

            foreach (var (modId, localMod) in LoadOrder) lbx_LoadOrder.Items.Add(localMod);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _modDatabase.WriteDatabase();
            _config.WriteConfiguration();
        }

        private async void txb_SaveFolder_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(txb_SaveFolder.Text)) _lastProcessedSave = null;

            async Task<bool> UserKeepsTyping()
            {
                var txt = txb_SaveFolder.Text;
                await Task.Delay(TypeDelay);

                return txt != txb_SaveFolder.Text;
            }

            if (await UserKeepsTyping() || txb_SaveFolder.Text == _lastProcessedSave) return;
            _lastProcessedSave = txb_SaveFolder.Text;

            var targetDir = @"" + _lastProcessedSave;

            if (!Directory.Exists(targetDir))
                return;

            _profileList = _config.ScanProfiles(brd_SaveFolderIndicator);

            if (_profileList != null && _profileList.Count > 0)
                _config.PopulateProfiles(cbx_ProfileSelect, _profileList.Keys);
        }

        private async void txb_ModFolder_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(txb_ModFolder.Text)) _lastProcessedMods = null;

            async Task<bool> UserKeepsTyping()
            {
                var txt = txb_SaveFolder.Text;
                await Task.Delay(TypeDelay);

                return txt != txb_ModFolder.Text;
            }

            if (await UserKeepsTyping() || txb_ModFolder.Text == _lastProcessedMods) return;
            _lastProcessedMods = txb_ModFolder.Text;

            var targetDir = @"" + _lastProcessedMods;

            if (!Directory.Exists(targetDir))
                return;

            _modList = _config.ScanMods(brd_ModFolderIndicator);
        }

        // DEBUG FUNCTIONALITY

        private void TestPOST1_Click(object sender, RoutedEventArgs e)
        {
            var resolveModTask = new Tasks.ResolveModTask(_modDatabase, _modList.Keys.First());
            var resolveModThread = new Thread(resolveModTask.Execute);
            resolveModThread.Start();
        }

        private void TestPOST2_Click(object sender, RoutedEventArgs e)
        {
            var resolveModsTask = new Tasks.ResolveModsTask(_modDatabase, _modList);
            var resolveModsThread = new Thread(resolveModsTask.Execute);
            resolveModsThread.Start();
        }

        private void TestREAD_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbx_ProfileSelect.Text))
                return;

            var loadProfileTask =
                new Tasks.LoadProfileTask(_modDatabase, _config.Properties.SaveFolderPath, cbx_ProfileSelect.Text);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();
        }

        private void TestWRITE_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbx_ProfileSelect.Text))
                return;

            var saveProfileTask =
                new Tasks.SaveProfileTask(_modDatabase, _config.Properties.SaveFolderPath, cbx_ProfileSelect.Text);
            var saveProfileThread = new Thread(saveProfileTask.Execute);
            saveProfileThread.Start();
        }
    }
}