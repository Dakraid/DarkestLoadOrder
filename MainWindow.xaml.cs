using DarkestLoadOrder.Threading;

namespace DarkestLoadOrder
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using ModUtility;

    using Newtonsoft.Json;

    using Ookii.Dialogs.Wpf;

    public class Configuration
    {
        public string SaveFolderPath { get; set; }

        // public Dictionary<string, string> ProfileList { get; set; }
        public string ModFolderPath { get; set; }
        // public Dictionary<string, string> ModList { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ConfigPath = @".\DarkestLoadOrder.json";
        private const int    TypeDelay  = 1000;

        private readonly Configuration _configuration;
        private readonly ModDatabase   _modDatabase = new();

        private Dictionary<string, string> _profileList;
        private Dictionary<ulong, string>  _modList;

        private string _lastProcessedSave;
        private string _lastProcessedMods;

        private Dictionary<string, string> ScanProfiles(string savePath)
        {
            if (string.IsNullOrWhiteSpace(savePath) || !Directory.Exists(savePath))
                return null;

            var files = Directory.GetFiles(savePath).ToList();

            if (!files.Select(Path.GetFileName).Contains("steam_init.json"))
            {
                MessageBox.Show(this, "The application could not find 'steam_init.json', please make sure you selected the right folder.");

                return null;
            }

            var directories = Directory.GetDirectories(savePath).Where(dir => dir.Contains("profile")).ToList();
            var profiles    = directories.Select(Path.GetFileName).ToList();

            if (profiles.Count == 0)
            {
                MessageBox.Show(this, "The application could not find any valid profile folders, please make sure you have selected the right folder and have a saved game.");

                return null;
            }

            brd_SaveFolderIndicator.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            return profiles.Zip(directories, (k, v) => new
                           {
                               k, v
                           })
                           .ToDictionary(x => x.k, x => x.v);
        }

        private Dictionary<ulong, string> ScanMods(string modPath)
        {
            if (string.IsNullOrWhiteSpace(modPath) || !Directory.Exists(modPath))
                return null;

            var directories = Directory.GetDirectories(modPath).ToList();
            var mods        = directories.Select(dir => ulong.Parse(Path.GetFileName(dir))).ToList();

            if (!(modPath.Contains("workshop") && modPath.Contains("262060")))
            {
                MessageBox.Show(this, "The application could not verify the workshop folder structure, please make sure you selected the right folder.");

                return null;
            }

            brd_ModFolderIndicator.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            return mods.Zip(directories, (k, v) => new
                       {
                           k, v
                       })
                       .ToDictionary(x => x.k, x => x.v);
        }

        private void PopulateProfiles(IReadOnlyCollection<string> profiles)
        {
            if (!profiles.Any())
                return;

            foreach (var profile in profiles)
                cbx_ProfileSelect.Items.Add(profile);
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

        public MainWindow()
        {
            _configuration = InitConfiguration();

            InitializeComponent();

            txb_SaveFolder.Text = _configuration.SaveFolderPath;
            txb_ModFolder.Text  = _configuration.ModFolderPath;

            _profileList = ScanProfiles(_configuration.SaveFolderPath);
            _modList     = ScanMods(_configuration.ModFolderPath);

            if (_profileList != null && _profileList.Count > 0)
                PopulateProfiles(_profileList.Keys);
        }

        private void btn_SelectSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.", UseDescriptionForTitle = true
            };

            // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");

            if (!(bool) dialog.ShowDialog(this))
                return;

            _configuration.SaveFolderPath = dialog.SelectedPath;
            _profileList                  = ScanProfiles(_configuration.SaveFolderPath);
            txb_SaveFolder.Text           = _configuration.SaveFolderPath;

            if (_profileList != null && _profileList.Count > 0)
                PopulateProfiles(_profileList.Keys);
        }

        private void btn_SelectModFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder.", UseDescriptionForTitle = true
            };

            // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");

            if (!(bool) dialog.ShowDialog(this))
                return;

            _configuration.ModFolderPath = dialog.SelectedPath;
            _modList                     = ScanMods(_configuration.ModFolderPath);
            txb_ModFolder.Text           = _configuration.ModFolderPath;
        }

        private void cbx_ProfileSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            brd_ProfileIndicator.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _modDatabase.WriteDatabase();
            WriteConfiguration();
        }

        private async void txb_SaveFolder_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
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

            _profileList = ScanProfiles(targetDir);

            if (_profileList != null && _profileList.Count > 0)
                PopulateProfiles(_profileList.Keys);
        }

        private async void txb_ModFolder_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
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

            _modList = ScanMods(targetDir);
        }

        private void TestPOST_Click(object sender, RoutedEventArgs e)
        {
            var resolveModsTask   = new Tasks.ResolveModsTask(_modDatabase, _modList);
            var resolveModsThread = new Thread(resolveModsTask.Execute);
            resolveModsThread.Start();

            _modDatabase.WriteDatabase();
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

            var saveProfileTask   = new Tasks.LoadProfileTask(_modDatabase, _configuration.SaveFolderPath, cbx_ProfileSelect.Text);
            var saveProfileThread = new Thread(saveProfileTask.Execute);
            saveProfileThread.Start();
        }
    }
}
