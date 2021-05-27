using System.IO;

namespace DarkestLoadOrder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using ModUtility;

    internal class Tasks
    {
        public class SaveProfileTask
        {
            private readonly ModDatabase _modDatabase;
            private readonly string      _profilePath;
            private readonly string      _selectedProfile;

            public SaveProfileTask(ModDatabase mdb, string pp, string sp)
            {
                _modDatabase     = mdb;
                _profilePath     = pp;
                _selectedProfile = sp;
            }

            // TryParseJSON() is a Rust function
            // It reads in a savedata_in.json and outputs a savedata_out.dson
            [DllImport("ddsavelib.dll")]
            private static extern void TryParseJSON();

            public void Execute()
            {
                // TODO
                if (!File.Exists(@".\savedata_in.json"))
                {
                    return;
                }

                TryParseJSON();

                // File.Copy(@".\savedata_out.dson", _profilePath + "\\" + _selectedProfile + "\\persist.game.json");
            }
        }

        public class LoadProfileTask
        {
            private readonly ModDatabase _modDatabase;
            private readonly string      _profilePath;
            private readonly string      _selectedProfile;

            public LoadProfileTask(ModDatabase mdb, string pp, string sp)
            {
                _modDatabase     = mdb;
                _profilePath     = pp;
                _selectedProfile = sp;
            }

            // TryParseBin() is a Rust function
            // It reads in a savedata_in.dson and outputs a savedata_out.json
            [DllImport("ddsavelib.dll")]
            private static extern void TryParseBin();

            public void Execute()
            {
                // TODO
                if (!File.Exists(_profilePath + "\\" + _selectedProfile + "\\persist.game.json"))
                {
                    return;
                }
                File.Copy(_profilePath + "\\" + _selectedProfile + "\\persist.game.json", @".\savedata_in.dson", true);

                TryParseBin();
            }
        }

        public class ResolveModsTask {
            private readonly ModDatabase               _modDatabase;
            private readonly Dictionary<ulong, string> _modList;

            public ResolveModsTask(ModDatabase mdb, Dictionary<ulong, string> ml)
            {
                _modDatabase = mdb;
                _modList     = ml;
            }

            public void Execute()
            {
                var modResolveOnline = new ModResolverOnline();

                foreach (var (mod, path) in _modList)
                {
                    var apiResponse = modResolveOnline.GetModInfo(mod);

                    var modItem = new ModItem
                    {
                        ModTitle       = apiResponse.publishedfiledetails[0].title,
                        ModDescription = apiResponse.publishedfiledetails[0].description
                    };

                    if (!_modDatabase.Mods.ContainsKey(mod))
                        _modDatabase.Mods.Add(mod, modItem);
                }
            }
        }
    }
}
