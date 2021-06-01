using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DarkestLoadOrder.ModHelper;

namespace DarkestLoadOrder.Threading
{
    internal class Tasks
    {
        public class SaveProfileTask
        {
            private readonly string _profilePath;
            private readonly string _selectedProfile;

            public SaveProfileTask(string pp, string sp)
            {
                _profilePath = pp;
                _selectedProfile = sp;
            }

            // TryParseJSON() is a Rust function
            // It reads in a savedata_in.json and outputs a savedata_out.dson
            [DllImport("ddsavelib.dll")]
            private static extern void TryParseJSON();

            public void Execute()
            {
                if (!File.Exists(@".\temp\savedata_in.json")) return;

                TryParseJSON();

                // File.Copy(@".\temp\savedata_out.dson", _profilePath + "\\" + _selectedProfile + "\\persist.game.json");
            }
        }

        public class LoadProfileTask
        {
            private readonly string _profilePath;
            private readonly string _selectedProfile;

            public LoadProfileTask(string pp, string sp)
            {
                _profilePath = pp;
                _selectedProfile = sp;
            }

            // TryParseBin() is a Rust function
            // It reads in a savedata_in.dson and outputs a savedata_out.json
            [DllImport("ddsavelib.dll")]
            private static extern void TryParseBin();

            public void Execute()
            {
                if (!File.Exists(_profilePath + "\\" + _selectedProfile + "\\persist.game.json")) return;

                if (!Directory.Exists(@".\temp\"))
                {
                    Directory.CreateDirectory(@".\temp\");
                }

                File.Copy(_profilePath + "\\" + _selectedProfile + "\\persist.game.json", @".\temp\savedata_in.dson", true);

                TryParseBin();
            }
        }
    }
}