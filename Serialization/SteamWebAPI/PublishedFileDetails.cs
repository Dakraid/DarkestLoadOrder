// --------------------------------------------------------------------------------------------------------------------
// Filename : PublishedFileDetails.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 28.05.2021 02:09
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

//    using DarkestLoadOrder.Json.SteamWebAPI;
//    var publishedFileDetails = PublishedFileDetails.FromJson(jsonString);

namespace DarkestLoadOrder.Serialization.SteamWebAPI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PublishedFileDetails
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public class Response
    {
        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("resultcount")]
        public long Resultcount { get; set; }

        [JsonProperty("publishedfiledetails")]
        public List<Publishedfiledetail> Publishedfiledetails { get; set; }
    }

    public class Publishedfiledetail
    {
        [JsonProperty("publishedfileid")]
        public ulong Publishedfileid { get; set; }

        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_app_id")]
        public long CreatorAppId { get; set; }

        [JsonProperty("consumer_app_id")]
        public long ConsumerAppId { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        [JsonProperty("hcontent_file")]
        public string HcontentFile { get; set; }

        [JsonProperty("preview_url")]
        public Uri PreviewUrl { get; set; }

        [JsonProperty("hcontent_preview")]
        public string HcontentPreview { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("time_created")]
        public long TimeCreated { get; set; }

        [JsonProperty("time_updated")]
        public long TimeUpdated { get; set; }

        [JsonProperty("visibility")]
        public long Visibility { get; set; }

        [JsonProperty("banned")]
        public long Banned { get; set; }

        [JsonProperty("ban_reason")]
        public string BanReason { get; set; }

        [JsonProperty("subscriptions")]
        public long Subscriptions { get; set; }

        [JsonProperty("favorited")]
        public long Favorited { get; set; }

        [JsonProperty("lifetime_subscriptions")]
        public long LifetimeSubscriptions { get; set; }

        [JsonProperty("lifetime_favorited")]
        public long LifetimeFavorited { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }
    }

    public class Tag
    {
        [JsonProperty("tag")]
        public string TagTag { get; set; }
    }

    public partial class PublishedFileDetails
    {
        public static PublishedFileDetails FromJson(string json)
        {
            return JsonConvert.DeserializeObject<PublishedFileDetails>(json, Converter.Settings);
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
                new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeUniversal
                }
            }
        };
    }
}
