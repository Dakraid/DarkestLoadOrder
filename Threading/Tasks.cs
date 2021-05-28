using System.IO;
using System.Linq;

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

            public async void Execute()
            {
                var apiResponse = ModResolverOnline.GetModInfos(_modList.Keys.ToArray());

                foreach (var publishedfiledetail in apiResponse.Publishedfiledetails)
                {
                    var modItem = new ModDatabaseItem
                    {
                        ModPublishedId = publishedfiledetail.Publishedfileid,
                        ModTitle       = publishedfiledetail.Title,
                        ModDescription = publishedfiledetail.Description,
                        ModThumbnail   = await ModResolverOnline.GetModThumbnail(publishedfiledetail.PreviewUrl)
                    };


                    if (!_modDatabase.KnownMods.ContainsKey(publishedfiledetail.Publishedfileid))
                        _modDatabase.KnownMods.Add(publishedfiledetail.Publishedfileid, modItem);
                }
            }
        }

        public class ResolveModTask {
            private readonly ModDatabase _modDatabase;
            private readonly ulong       _mod;

            public ResolveModTask(ModDatabase mdb, ulong md)
            {
                _modDatabase = mdb;
                _mod         = md;
            }

            public async void Execute()
            {
                var apiResponse      = ModResolverOnline.GetModInfo(_mod);
                
                var modItem = new ModDatabaseItem
                {
                    ModPublishedId = apiResponse.Publishedfiledetails[0].Publishedfileid,
                    ModTitle       = apiResponse.Publishedfiledetails[0].Title,
                    ModDescription = apiResponse.Publishedfiledetails[0].Description,
                    ModThumbnail   = await ModResolverOnline.GetModThumbnail(apiResponse.Publishedfiledetails[0].PreviewUrl)
                };

                if (!_modDatabase.KnownMods.ContainsKey(_mod))
                    _modDatabase.KnownMods.Add(_mod, modItem);
            }
        }
    }
}
