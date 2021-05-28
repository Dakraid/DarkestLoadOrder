//    using ModUtility;
//    var modDatabaseItem = ModDatabaseItem.FromJson(jsonString);

using DarkestLoadOrder.Json.Savegame;

namespace DarkestLoadOrder.ModUtility
{
    public class ModDatabaseItem
    {
        public ulong  ModPublishedId { get; set; }
        public string ModTitle       { get; set; }
        public string ModDescription { get; set; }
        public byte[] ModThumbnail   { get; set; }
    }

    public class ModLocalItem : ModDatabaseItem
    {
        public ModLocalItem(ModDatabaseItem modItem = null)
        {
            if (modItem == null) return;

            ModPublishedId = modItem.ModPublishedId;
            ModTitle       = modItem.ModTitle;
            ModDescription = modItem.ModDescription;
            ModThumbnail   = modItem.ModThumbnail;
        }

        public bool  ModEnabled  { get; set; }
        public ulong ModPriority { get; set; }
        public Source ModSource { get; set; }
    }
}
