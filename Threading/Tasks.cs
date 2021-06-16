// --------------------------------------------------------------------------------------------------------------------
// Filename : Tasks.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 25.05.2021 23:25
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;

    internal class Tasks
    {
        public class ProfileScanTask
        {
            public static async Task<Dictionary<string, string>> Execute(string savePath)
            {
                return await Task.Run(() => {
                    if (string.IsNullOrWhiteSpace(savePath) || !Directory.Exists(savePath))
                        return null;

                    var files = Directory.GetFiles(savePath).ToList();

                    if (!files.Select(Path.GetFileName).Contains("steam_init.json"))
                    {
                        MessageBox.Show("The application could not find 'steam_init.json', please make sure you selected the right folder.");

                        return null;
                    }

                    var directories = Directory.GetDirectories(savePath).Where(dir => dir.Contains("profile")).ToList();
                    var profiles = directories.Select(d => (string) Path.GetFileName(d) ).ToList();

                    if (profiles.Count != 0)
                        return profiles.Zip(directories, (k, v) => new
                                       {
                                           k, v
                                       })
                                       .ToDictionary(x => x.k, x => x.v);

                    MessageBox.Show("The application could not find any valid profile folders, please make sure you have selected the right folder and have a saved game.");
                    return null;
                });
            }
        }

        public class SaveProfileTask
        {
            private readonly string _profilePath;
            private readonly string _selectedProfile;

            public SaveProfileTask(string profilePath, string selectedProfile)
            {
                _profilePath     = profilePath;
                _selectedProfile = selectedProfile;
            }

            // TryParseJSON() is a Rust function
            // It reads in a savedata_out.json and outputs a savedata_out.dson
            [DllImport("ddsavelib.dll")]
            private static extern void TryParseJSON();

            public void Execute()
            {
                if (!File.Exists(@".\temp\savedata_out.json")) return;

                try
                {
                    TryParseJSON();
                }
                catch (Exception e)
                {
                    MessageBox.Show("The program ran into an error:\n" + e.Message);

                    return;
                }

                var targetFile = _profilePath + "\\" + _selectedProfile + "\\persist.game.json";

                File.Copy(targetFile, targetFile + ".bak", true);
                File.Copy(@".\temp\savedata_out.dson", targetFile, true);

                MessageBox.Show("Your changes have been saved!");
            }
        }

        public class LoadProfileTask
        {
            private readonly string _profilePath;
            private readonly string _selectedProfile;

            public LoadProfileTask(string profilePath, string selectedProfile)
            {
                _profilePath     = profilePath;
                _selectedProfile = selectedProfile;
            }

            // TryParseBin() is a Rust function
            // It reads in a savedata_in.dson and outputs a savedata_in.json
            [DllImport("ddsavelib.dll")]
            private static extern void TryParseBin();

            public void Execute()
            {
                var targetFile = _profilePath + "\\" + _selectedProfile + "\\persist.game.json";

                if (!File.Exists(targetFile)) return;

                if (!Directory.Exists(@".\temp\"))
                    Directory.CreateDirectory(@".\temp\");

                File.Copy(targetFile, @".\temp\savedata_in.dson", true);

                try
                {
                    TryParseBin();
                }
                catch (Exception e)
                {
                    MessageBox.Show("The program ran into an error:\n" + e.Message);
                }
            }
        }
    }
}
