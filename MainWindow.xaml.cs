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
using DarkestLoadOrder.ModUtility;
using DarkestLoadOrder.Threading;
using DarkestLoadOrder.Utility;

using Newtonsoft.Json;

using Ookii.Dialogs.Wpf;

namespace DarkestLoadOrder
{
    public class Configuration
    {
        public string SaveFolderPath { get; set; }

        // public Dictionary<string, string> ProfileList { get; set; }
        public string ModFolderPath { get; set; }
        // public Dictionary<string, string> ModList { get; set; }
    }

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ConfigPath = @".\DarkestLoadOrder.json";
        private const int    TypeDelay  = 1000;

        private readonly Configuration _configuration;
        private readonly ModDatabase   _modDatabase = new();

        private          string        _lastProcessedMods;
        private          string        _lastProcessedSave;

        public  Dictionary<ulong, string>  _modList;
        public  Dictionary<string, string> _profileList;

        public int ActiveModCount => 16;
        public int TotalModCount  => 35;

        public MainWindow()
        {
            _configuration = InitConfiguration();

            InitializeComponent();

            txb_SaveFolder.Text = _configuration.SaveFolderPath;
            txb_ModFolder.Text  = _configuration.ModFolderPath;

            _profileList = Scanner.ScanProfiles(_configuration.SaveFolderPath, brd_SaveFolderIndicator);
            _modList     = Scanner.ScanMods(_configuration.ModFolderPath, brd_ModFolderIndicator);

            if (_profileList != null && _profileList.Count > 0)
                Scanner.PopulateProfiles(cbx_ProfileSelect, _profileList.Keys);
        }

        private Configuration InitConfiguration()
        {
            if (!File.Exists(ConfigPath))
            {
                File.Create(ConfigPath);

                return new Configuration();
            }

            var tempConfig = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath));

            if (tempConfig == null)
            {
                MessageBox.Show(this, "Could not deserialize the configuration file, creating anew.");

                return new Configuration();
            }

            var savePath = Directory.Exists(tempConfig.SaveFolderPath);
            var modPath  = Directory.Exists(tempConfig.ModFolderPath);

            if (savePath && modPath) return tempConfig;

            MessageBox.Show(this, $"Verification of the data paths failed. Please reselect the failed path.\nSave Folder is valid: {savePath}\nMod Folder is valid: {modPath}");

            if (!savePath)
                tempConfig.SaveFolderPath = null;

            if (!modPath)
                tempConfig.ModFolderPath = null;

            return tempConfig;
        }

        private void WriteConfiguration()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_configuration));
        }

        private void btn_SelectSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.", UseDescriptionForTitle = true
            };

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog(this))
                return;

            _configuration.SaveFolderPath = dialog.SelectedPath;
            _profileList                  = Scanner.ScanProfiles(_configuration.SaveFolderPath, brd_SaveFolderIndicator);
            txb_SaveFolder.Text           = _configuration.SaveFolderPath;

            if (_profileList != null && _profileList.Count > 0)
                Scanner.PopulateProfiles(cbx_ProfileSelect, _profileList.Keys);
        }

        private void btn_SelectModFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.", UseDescriptionForTitle = true
            };

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");

            // ReSharper disable once PossibleInvalidOperationException
            if (!(bool) dialog.ShowDialog(this))
                return;

            _configuration.ModFolderPath = dialog.SelectedPath;
            _modList                     = Scanner.ScanMods(_configuration.ModFolderPath, brd_ModFolderIndicator);
            txb_ModFolder.Text           = _configuration.ModFolderPath;
        }

        private void cbx_ProfileSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            brd_ProfileIndicator.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            var newSelect = e.AddedItems[0]?.ToString();

            if (string.IsNullOrWhiteSpace(newSelect))
                return;

            var loadProfileTask   = new Tasks.LoadProfileTask(_modDatabase, _configuration.SaveFolderPath, newSelect);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();

            while (loadProfileThread.IsAlive)
                Thread.Sleep(500);

            var saveData = SaveData.FromJson(File.ReadAllText(@".\savedata_out.json"));

            foreach (var modUGC in saveData.BaseRoot.AppliedUgcs1_0)
            {
                var modId = ulong.Parse(modUGC.Value.Name);
                _modDatabase.KnownMods.TryGetValue(modId, out var modItem);

                var localMod = new ModLocalItem(modItem)
                {
                    ModEnabled = true, ModPriority = ulong.Parse(modUGC.Key), ModSource = modUGC.Value.Source
                };

                _modDatabase.ProfileMods.Add(modId, localMod);
                lbx_LoadOrder.Items.Add(localMod);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _modDatabase.WriteDatabase();
            WriteConfiguration();
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

            _profileList = Scanner.ScanProfiles(targetDir, brd_SaveFolderIndicator);

            if (_profileList != null && _profileList.Count > 0)
                Scanner.PopulateProfiles(cbx_ProfileSelect, _profileList.Keys);
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

            _modList = Scanner.ScanMods(targetDir, brd_ModFolderIndicator);
        }

        // DEBUG FUNCTIONALITY

        private void TestPOST1_Click(object sender, RoutedEventArgs e)
        {
            var resolveModTask   = new Tasks.ResolveModTask(_modDatabase, _modList.Keys.First());
            var resolveModThread = new Thread(resolveModTask.Execute);
            resolveModThread.Start();
        }

        private void TestPOST2_Click(object sender, RoutedEventArgs e)
        {
            var resolveModsTask   = new Tasks.ResolveModsTask(_modDatabase, _modList);
            var resolveModsThread = new Thread(resolveModsTask.Execute);
            resolveModsThread.Start();
        }

        private void TestREAD_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbx_ProfileSelect.Text))
                return;

            var loadProfileTask   = new Tasks.LoadProfileTask(_modDatabase, _configuration.SaveFolderPath, cbx_ProfileSelect.Text);
            var loadProfileThread = new Thread(loadProfileTask.Execute);
            loadProfileThread.Start();
        }

        private void TestWRITE_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cbx_ProfileSelect.Text))
                return;

            var saveProfileTask   = new Tasks.SaveProfileTask(_modDatabase, _configuration.SaveFolderPath, cbx_ProfileSelect.Text);
            var saveProfileThread = new Thread(saveProfileTask.Execute);
            saveProfileThread.Start();
        }
    }
}
