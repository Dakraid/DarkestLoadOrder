//    using DarkestLoadOrder.Json.Savegame;
//    var saveData = SaveData.FromJson(jsonString);

using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DarkestLoadOrder.Json.Savegame
{
    public partial class SaveData
    {
        [JsonProperty("__revision_dont_touch", Required = Required.Always)]
        public long RevisionDontTouch { get; set; }

        [JsonProperty("base_root", Required = Required.Always)]
        public BaseRoot BaseRoot { get; set; }
    }

    public class BaseRoot
    {
        [JsonProperty("version", Required = Required.Always)]
        public long Version { get; set; }

        [JsonProperty("totalelapsed", Required = Required.Always)]
        public long Totalelapsed { get; set; }

        [JsonProperty("inraid", Required = Required.Always)]
        public bool Inraid { get; set; }

        [JsonProperty("raiddungeon", Required = Required.Always)]
        public string Raiddungeon { get; set; }

        [JsonProperty("raid_save", Required = Required.Always)]
        public string RaidSave { get; set; }

        [JsonProperty("estatename", Required = Required.Always)]
        public string Estatename { get; set; }

        [JsonProperty("game_mode", Required = Required.Always)]
        public string GameMode { get; set; }

        [JsonProperty("date_time", Required = Required.Always)]
        public string DateTime { get; set; }

        [JsonProperty("dd_options_altered", Required = Required.Always)]
        public bool DdOptionsAltered { get; set; }

        [JsonProperty("profile_options", Required = Required.Always)]
        public ProfileOptions ProfileOptions { get; set; }

        [JsonProperty("applied_ugcs_1_0", Required = Required.Always)]
        public Dictionary<string, AppliedUgcs1_0> AppliedUgcs1_0 { get; set; }

        [JsonProperty("persistent_ugcs", Required = Required.Always)]
        public PersistentUgcs PersistentUgcs { get; set; }

        [JsonProperty("presented_dlc", Required = Required.Always)]
        public PersistentUgcs PresentedDlc { get; set; }

        [JsonProperty("dlc_init", Required = Required.Always)]
        public bool DlcInit { get; set; }
    }

    public class AppliedUgcs1_0
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("source", Required = Required.Always)]
        public Source Source { get; set; }
    }

    public class PersistentUgcs { }

    public class ProfileOptions
    {
        [JsonProperty("values", Required = Required.Always)]
        public Values Values { get; set; }
    }

    public class Values
    {
        [JsonProperty("quest_select_warnings", Required = Required.Always)]
        public bool[] QuestSelectWarnings { get; set; }

        [JsonProperty("provision_warnings", Required = Required.Always)]
        public bool[] ProvisionWarnings { get; set; }

        [JsonProperty("deck_based_stage_coach", Required = Required.Always)]
        public bool[] DeckBasedStageCoach { get; set; }

        [JsonProperty("curio_tracker", Required = Required.Always)]
        public bool[] CurioTracker { get; set; }

        [JsonProperty("dd_mode", Required = Required.Always)]
        public bool[] DdMode { get; set; }

        [JsonProperty("corpses", Required = Required.Always)]
        public bool[] Corpses { get; set; }

        [JsonProperty("stall_penalty", Required = Required.Always)]
        public bool[] StallPenalty { get; set; }

        [JsonProperty("deaths_door_recovery_debuffs", Required = Required.Always)]
        public bool[] DeathsDoorRecoveryDebuffs { get; set; }

        [JsonProperty("retreats_can_fail", Required = Required.Always)]
        public bool[] RetreatsCanFail { get; set; }

        [JsonProperty("multiplied_enemy_crits", Required = Required.Always)]
        public bool[] MultipliedEnemyCrits { get; set; }

        [JsonProperty("town_events", Required = Required.Always)]
        public string TownEvents { get; set; }

        [JsonProperty("never_again", Required = Required.Always)]
        public string NeverAgain { get; set; }
    }

    public enum Source
    {
        Steam
    };

    public partial class SaveData
    {
        public static SaveData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SaveData>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this SaveData self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling        = DateParseHandling.None,
            Converters =
            {
                SourceConverter.Singleton,
                new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeUniversal
                }
            }
        };
    }

    internal class SourceConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(Source) || t == typeof(Source?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);

            if (value == "Steam")
                return Source.Steam;

            throw new Exception("Cannot unmarshal type Source");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);

                return;
            }
            var value = (Source) untypedValue;

            if (value == Source.Steam)
            {
                serializer.Serialize(writer, "Steam");

                return;
            }

            throw new Exception("Cannot marshal type Source");
        }

        public static readonly SourceConverter Singleton = new();
    }
}
