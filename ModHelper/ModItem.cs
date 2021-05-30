//    using ModUtility;
//    var modDatabaseItem = ModDatabaseItem.FromJson(jsonString);

namespace DarkestLoadOrder.ModHelper
{
    public class ModItem
    {
        public ulong ModPublishedId { get; set; }
        public string ModTitle { get; set; }
    }

    public class ModLocalItem : ModItem
    {
        public string ModDescription { get; set; }
        public byte[] ModThumbnail { get; set; } = null;
        public bool ModEnabled { get; set; } = false;
        public long ModPriority { get; set; } = -1;
        public string ModSource { get; set; } = "Unknown";
    }

    public class ModDatabaseItem : ModItem
    {
        public string ModType { get; set; }
        public long ModPriority { get; set; }
    }
}