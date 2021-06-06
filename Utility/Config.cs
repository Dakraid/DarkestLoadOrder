namespace DarkestLoadOrder.Utility
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Newtonsoft.Json;

    public class Store
    {
        public string SaveFolderPath { get; set; } = "";
        public string ModFolderPath  { get; set; } = "";
    }

    public class Config
    {
        private const string ConfigPath = @".\DarkestLoadOrder.json";

        public Store Properties = new();

        public Config()
        {
            if (!File.Exists(ConfigPath))
            {
                File.Create(ConfigPath);

                return;
            }

            var tempConfig = JsonConvert.DeserializeObject<Store>(File.ReadAllText(ConfigPath));

            if (tempConfig == null)
            {
                MessageBox.Show("Could not read the configuration file, creating new.");

                return;
            }

            var savePath = Directory.Exists(tempConfig.SaveFolderPath);
            var modPath  = Directory.Exists(tempConfig.ModFolderPath);

            if (!savePath || !modPath)
                MessageBox.Show($"Verification of the data paths failed. Please reselect the failed path.\nSave Folder is valid: {savePath}\nMod Folder is valid: {modPath}");

            Properties = tempConfig;
        }

        public void WriteConfiguration()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Properties));
        }

        public Dictionary<string, string> ScanProfiles(Border targetBorder = null)
        {
            var savePath = Properties.SaveFolderPath;

            if (string.IsNullOrWhiteSpace(savePath) || !Directory.Exists(savePath))
                return null;

            var files = Directory.GetFiles(savePath).ToList();

            if (!files.Select(Path.GetFileName).Contains("steam_init.json"))
            {
                MessageBox.Show("The application could not find 'steam_init.json', please make sure you selected the right folder.");

                return null;
            }

            var directories = Directory.GetDirectories(savePath).Where(dir => dir.Contains("profile")).ToList();
            var profiles    = directories.Select(Path.GetFileName).ToList();

            if (profiles.Count == 0)
            {
                MessageBox.Show("The application could not find any valid profile folders, please make sure you have selected the right folder and have a saved game.");

                return null;
            }

            if (targetBorder != null)
                targetBorder.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            return profiles.Zip(directories, (k, v) => new
                           {
                               k, v
                           })
                           .ToDictionary(x => x.k, x => x.v);
        }

        public Dictionary<ulong, string> ScanMods(Border targetBorder = null)
        {
            var modPath = Properties.ModFolderPath;

            if (string.IsNullOrWhiteSpace(modPath) || !Directory.Exists(modPath))
                return null;

            var directories = Directory.GetDirectories(modPath).ToList();
            var mods        = directories.Select(dir => ulong.Parse(Path.GetFileName(dir))).ToList();

            if (!(modPath.Contains("workshop") && modPath.Contains("262060")))
            {
                MessageBox.Show("The application could not verify the workshop folder structure, please make sure you selected the right folder.");

                return null;
            }

            if (targetBorder != null)
                targetBorder.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            return mods.Zip(directories, (k, v) => new
                       {
                           k, v
                       })
                       .ToDictionary(x => x.k, x => x.v);
        }
    }
}
