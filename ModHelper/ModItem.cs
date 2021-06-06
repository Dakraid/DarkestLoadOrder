// --------------------------------------------------------------------------------------------------------------------
// Filename : ModItem.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

//    using ModUtility;
//    var modDatabaseItem = ModDatabaseItem.FromJson(jsonString);

namespace DarkestLoadOrder.ModHelper
{
    public class ModItem
    {
        public ulong ModPublishedId { get; set; }
        public string ModTitle { get; set; }
        public string ModDescription { get; set; }
        public byte[] ModThumbnail { get; set; }
    }

    public class ModLocalItem : ModItem
    {
        public long ModPriority { get; set; } = -1;
        public string ModSource { get; set; } = "Unknown";

        public static implicit operator ModLocalItem(ModDatabaseItem x)
        {
            return new()
            {
                ModPublishedId = x.ModPublishedId, ModTitle = x.ModTitle, ModDescription = x.ModDescription, ModThumbnail = x.ModThumbnail
            };
        }
    }

    public class ModDatabaseItem : ModItem
    {
        public ModType ModType { get; set; } = ModType.Unknown;
        public long ModTypePriority { get; set; } = 0;

        public static implicit operator ModDatabaseItem(ModLocalItem x)
        {
            return new()
            {
                ModPublishedId = x.ModPublishedId, ModTitle = x.ModTitle, ModDescription = x.ModDescription, ModThumbnail = x.ModThumbnail
            };
        }
    }

    public enum ModType
    {
        Unknown,
        Class,
        UserInterface,
        Patch
    }
}
