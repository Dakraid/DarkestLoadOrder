// --------------------------------------------------------------------------------------------------------------------
// Filename : ModDatabase.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.ModHelper
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    using Utility;

    using ViewModel;

    public class ModDatabase
    {
        private const string DbPath = @".\DarkestLoadOrder.database.json";
        private readonly ModernApplicationViewModel _parentContext;

        public ModDatabase(ModernApplicationViewModel context)
        {
            _parentContext = context;

            ReadDatabase();
        }

        public async void ReadMods()
        {
            HashSet<ulong> knownMods = null;

            if (_parentContext.Application.LocalMods?.Count > 0)
            {
                knownMods = new HashSet<ulong>();

                foreach (var (modId, _) in _parentContext.Application.LocalMods)
                {
                    knownMods.Add(modId);
                }
            }

            Dictionary<ulong, ModLocalItem> newMods;

            if (knownMods?.Count > 0)
                newMods = await ModResolverOnline.GetModInfos(_parentContext.Application.Settings.ModFolderPath, knownMods);
            else
                newMods = await ModResolverOnline.GetModInfos(_parentContext.Application.Settings.ModFolderPath);
            
            _parentContext.Application.LocalMods ??= new ObservableDictionary<ulong, ModLocalItem>();

            newMods?.ToList().ForEach(x => _parentContext.Application.LocalMods[x.Key] = x.Value);

            _parentContext.Application.ModsLoaded = true;
        }

        public void ReadDatabase()
        {
            if (!File.Exists(DbPath))
                return;

            var tempItems = JsonConvert.DeserializeObject<Dictionary<ulong, ModLocalItem>>(File.ReadAllText(DbPath));

            if (tempItems == null || tempItems.Count == 0)
                return;

            _parentContext.Application.LocalMods ??= new ObservableDictionary<ulong, ModLocalItem>();

            if (_parentContext.Application.LocalMods.Count > 0)
            {
                tempItems.ToList().ForEach(x => _parentContext.Application.LocalMods[x.Key] = x.Value);
            }
            else
            {
                _parentContext.Application.LocalMods.AddRange(tempItems);
            }

            foreach (var localMod in _parentContext.Application.LocalMods)
            {
                localMod.Value.ModPriority = -1;
            }

            _parentContext.Application.ModsLoaded = true;
        }

        public void WriteDatabase()
        {
            File.WriteAllText(DbPath, JsonConvert.SerializeObject(_parentContext.Application.LocalMods));
        }
    }
}
