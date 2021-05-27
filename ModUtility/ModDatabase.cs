namespace DarkestLoadOrder.ModUtility
{
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    public class ModItem
    {
        public string ModTitle       { get; set; }
        public string ModDescription { get; set; }
    }

    public class ModDatabase
    {
        private const string dbPath = @".\DarkestLoadOrder.database.json";

        public Dictionary<ulong, ModItem> Mods = new();

        public ModDatabase()
        {
            if (!File.Exists(dbPath))
            {
                File.Create(dbPath);

                return;
            }

            Mods = JsonConvert.DeserializeObject<Dictionary<ulong, ModItem>>(File.ReadAllText(dbPath));
        }

        public void WriteDatabase()
        {
            File.WriteAllText(dbPath, JsonConvert.SerializeObject(Mods));
        }
    }
}
