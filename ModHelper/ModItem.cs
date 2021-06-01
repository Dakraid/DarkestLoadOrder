//    using ModUtility;
//    var modDatabaseItem = ModDatabaseItem.FromJson(jsonString);

namespace DarkestLoadOrder.ModHelper
{
    public class ModItem
    {
        public ulong  ModPublishedId { get; set; }
        public string ModTitle       { get; set; }
        public string ModDescription { get; set; }
        public byte[] ModThumbnail   { get; set; } = null;
    }

    public class ModLocalItem : ModItem
    {
        public static implicit operator ModLocalItem(ModDatabaseItem x)
        {
            return new() { ModPublishedId = x.ModPublishedId, ModTitle = x.ModTitle, ModDescription = x.ModDescription, ModThumbnail = x.ModThumbnail };
        }

        public bool ModEnabled { get; set; } = false;
        public long ModPriority { get; set; } = -1;
        public string ModSource { get; set; } = "Unknown";
    }

    public class ModDatabaseItem : ModItem
    {
        public static implicit operator ModDatabaseItem(ModLocalItem x)
        {
            return new() { ModPublishedId = x.ModPublishedId, ModTitle = x.ModTitle, ModDescription = x.ModDescription, ModThumbnail = x.ModThumbnail };
        }

        public ModType ModType         { get; set; } = ModType.Unknown;
        public long    ModTypePriority { get; set; } = 0;
    }

    public enum ModType
    {
        Unknown,
        Class,
        UserInterface,
        Patch
    }
}