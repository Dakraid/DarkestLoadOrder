using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace DarkestLoadOrder.ModUtility
{
    public class ModDatabase
    {
        private const string dbPath = @".\DarkestLoadOrder.database.json";

        private string _databaseText;

        public Dictionary<ulong, ModDatabaseItem> KnownMods   = new();
        public Dictionary<ulong, ModLocalItem>    ProfileMods = new();

        public ModDatabase()
        {
            ReadDatabase();
        }

        public async void ReadDatabase()
        {
            if (!File.Exists(dbPath))
                return;

            _databaseText = await File.ReadAllTextAsync(dbPath);

            var tempItems = JsonConvert.DeserializeObject<Dictionary<ulong, ModDatabaseItem>>(_databaseText);

            if (tempItems == null || tempItems.Count == 0)
                return;

            KnownMods = tempItems;
        }

        public void WriteDatabase()
        {
            File.WriteAllText(dbPath, JsonConvert.SerializeObject(KnownMods));
        }
    }
}
