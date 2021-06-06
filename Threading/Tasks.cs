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
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal class Tasks
    {
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
