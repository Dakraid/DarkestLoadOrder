using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DarkestLoadOrder.ModHelper
{
    public class ModDatabase
    {
        private const string DBPath = @".\DarkestLoadOrder.database.json";

        private string _databaseText;

        public Dictionary<ulong, ModDatabaseItem> KnownMods = new();

        public ModDatabase()
        {
            ReadDatabase();
        }

        public void ReadDatabase()
        {
            if (!File.Exists(DBPath))
                return;

            _databaseText = File.ReadAllText(DBPath);

            var tempItems = JsonConvert.DeserializeObject<Dictionary<ulong, ModDatabaseItem>>(_databaseText);

            if (tempItems == null || tempItems.Count == 0)
                return;

            KnownMods = tempItems;
        }

        public void WriteDatabase()
        {
            File.WriteAllText(DBPath, JsonConvert.SerializeObject(KnownMods));
        }
    }
}