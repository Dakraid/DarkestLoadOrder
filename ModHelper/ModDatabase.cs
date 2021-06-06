using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DarkestLoadOrder.ModHelper
{
    using System.Linq;
    using System.Windows.Documents;

    public class ModDatabase
    {
        private const string DBPath = @".\DarkestLoadOrder.database.json";

        private string _databaseText;

        public Dictionary<ulong, ModDatabaseItem> KnownMods = new();

        public ModDatabase()
        {
            ReadDatabase();
        }

        public void ReadDatabase(string modFolderPath = "")
        {
            if (!File.Exists(DBPath))
                return;

            _databaseText = File.ReadAllText(DBPath);

            var tempItems = JsonConvert.DeserializeObject<Dictionary<ulong, ModDatabaseItem>>(_databaseText);

            if (tempItems == null || tempItems.Count == 0)
                return;

            List<ulong> existingMods = new();

            if (!string.IsNullOrWhiteSpace(modFolderPath)) 
            {
                var directories = Directory.GetDirectories(modFolderPath);
                if (directories.Length != 0)
                {
                    foreach (var directory in directories)
                    {
                        var files = Directory.GetFiles(directory).ToHashSet();

                        if (!files.TryGetValue(directory + "\\project.xml", out var projectFile))
                            continue;

                        var modId = Path.GetFileName(directory);
                        existingMods.Add(ulong.Parse(modId));
                    }

                    tempItems = tempItems.Where(x => existingMods.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                }
            }

            KnownMods = tempItems;
        }

        public void WriteDatabase()
        {
            File.WriteAllText(DBPath, JsonConvert.SerializeObject(KnownMods));
        }
    }
}