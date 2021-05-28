using System.Windows.Controls;

namespace DarkestLoadOrder.Utility
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    using DarkestLoadOrder.ModUtility;

    using Newtonsoft.Json;

    using Ookii.Dialogs.Wpf;

    class Scanner
    {
        public static Dictionary<string, string> ScanProfiles(string savePath, Border targetBorder)
        {
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
            
            targetBorder.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            return profiles.Zip(directories, (k, v) => new
                           {
                               k, v
                           })
                           .ToDictionary(x => x.k, x => x.v);
        }

        public static Dictionary<ulong, string> ScanMods(string modPath, Border targetBorder)
        {
            if (string.IsNullOrWhiteSpace(modPath) || !Directory.Exists(modPath))
                return null;

            var directories = Directory.GetDirectories(modPath).ToList();
            var mods        = directories.Select(dir => ulong.Parse(Path.GetFileName(dir))).ToList();

            if (!(modPath.Contains("workshop") && modPath.Contains("262060")))
            {
                MessageBox.Show("The application could not verify the workshop folder structure, please make sure you selected the right folder.");

                return null;
            }

            targetBorder.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));

            return mods.Zip(directories, (k, v) => new
                       {
                           k, v
                       })
                       .ToDictionary(x => x.k, x => x.v);
        }

        public static void PopulateProfiles(ComboBox targetComboBox, IReadOnlyCollection<string> profiles)
        {
            if (!profiles.Any())
                return;

            foreach (var profile in profiles)
                targetComboBox.Items.Add(profile);
        }
    }
}
