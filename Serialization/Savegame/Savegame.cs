// --------------------------------------------------------------------------------------------------------------------
// Filename : Savegame.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 28.05.2021 02:09
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

//    using DarkestLoadOrder.Serialization.Savegame;
//    var saveData = SaveData.FromJson(jsonString);

namespace DarkestLoadOrder.Serialization.Savegame
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class SaveData
    {
        [JsonProperty("__revision_dont_touch")]
        public long RevisionDontTouch { get; set; }

        [JsonProperty("base_root")]
        public BaseRoot BaseRoot { get; set; }
    }

    public class BaseRoot
    {
        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("totalelapsed")]
        public long Totalelapsed { get; set; }

        [JsonProperty("inraid")]
        public bool Inraid { get; set; }

        [JsonProperty("raiddungeon")]
        public string Raiddungeon { get; set; }

        [JsonProperty("raid_save")]
        public string RaidSave { get; set; }

        [JsonProperty("estatename")]
        public string Estatename { get; set; }

        [JsonProperty("game_mode")]
        public string GameMode { get; set; }

        [JsonProperty("date_time")]
        public string DateTime { get; set; }

        [JsonProperty("dd_options_altered")]
        public bool DdOptionsAltered { get; set; }

        [JsonProperty("profile_options")]
        public ProfileOptions ProfileOptions { get; set; }

        [JsonProperty("applied_ugcs_1_0")]
        public Dictionary<string, AppliedUgcs1_0> AppliedUgcs1_0 { get; set; }

        [JsonProperty("persistent_ugcs")]
        public PersistentUgcs PersistentUgcs { get; set; }

        [JsonProperty("presented_dlc")]
        public PersistentUgcs PresentedDlc { get; set; }

        [JsonProperty("dlc_init")]
        public bool DlcInit { get; set; }

        [JsonProperty("dlc")]
        public Dictionary<string, AppliedUgcs1_0> Dlc { get; set; }
    }

    public class AppliedUgcs1_0
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }

    public class PersistentUgcs { }

    public class ProfileOptions
    {
        [JsonProperty("values")]
        public Values Values { get; set; }
    }

    public class Values
    {
        [JsonProperty("quest_select_warnings")]
        public List<bool> QuestSelectWarnings { get; set; }

        [JsonProperty("provision_warnings")]
        public List<bool> ProvisionWarnings { get; set; }

        [JsonProperty("deck_based_stage_coach")]
        public List<bool> DeckBasedStageCoach { get; set; }

        [JsonProperty("curio_tracker")]
        public List<bool> CurioTracker { get; set; }

        [JsonProperty("dd_mode")]
        public List<bool> DdMode { get; set; }

        [JsonProperty("corpses")]
        public List<bool> Corpses { get; set; }

        [JsonProperty("stall_penalty")]
        public List<bool> StallPenalty { get; set; }

        [JsonProperty("deaths_door_recovery_debuffs")]
        public List<bool> DeathsDoorRecoveryDebuffs { get; set; }

        [JsonProperty("retreats_can_fail")]
        public List<bool> RetreatsCanFail { get; set; }

        [JsonProperty("multiplied_enemy_crits")]
        public List<bool> MultipliedEnemyCrits { get; set; }

        [JsonProperty("town_events")]
        public string TownEvents { get; set; }

        [JsonProperty("never_again")]
        public string NeverAgain { get; set; }
    }

    public partial class SaveData
    {
        public static SaveData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SaveData>(json);
        }
    }
}
