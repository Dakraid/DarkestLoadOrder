// --------------------------------------------------------------------------------------------------------------------
// Filename : ModResolver_Online.cs
// Project: DarkestLoadOrder / DarkestLoadOrder
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 31.05.2021 00:13
// Last Modified On : 06.06.2021 14:38
// Copyrights : Copyright (c) Kristian Schlikow 2021-2021, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace DarkestLoadOrder.ModHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Serialization.SteamWebAPI;

    public class ModResolverOnline
    {
        private const string RequestUri =
            "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";

        public static async Task<byte[]> GetModThumbnail(Uri thumbnailUri)
        {
            using var httpClient = new HttpClient();
            var       imageBytes = await httpClient.GetByteArrayAsync(thumbnailUri);

            return imageBytes;
        }

        public static async Task<Dictionary<ulong, ModLocalItem>> GetModInfos(string modFolderPath, HashSet<ulong> knownMods)
        {
            var directories = Directory.GetDirectories(modFolderPath);

            if (directories.Length == 0)
                return null;

            List<ulong> modIDs = new();

            foreach (var directory in directories)
                if (ulong.TryParse(Path.GetFileName(directory), out var modId) && !knownMods.Contains(modId))
                    modIDs.Add(modId);

            return await GetModInfos(modIDs.ToArray());
        }


        public static async Task<Dictionary<ulong, ModLocalItem>> GetModInfos(string modFolderPath)
        {
            var directories = Directory.GetDirectories(modFolderPath);

            if (directories.Length == 0)
                return null;

            List<ulong> modIDs = new();

            foreach (var directory in directories)
                if (ulong.TryParse(Path.GetFileName(directory), out var modId))
                    modIDs.Add(modId);

            return await GetModInfos(modIDs.ToArray());
        }

        public static async Task<Dictionary<ulong, ModLocalItem>> GetModInfos(ulong[] modIDs)
        {
            if (modIDs.Length == 0) return null;

            var postDataBuilder = new StringBuilder();
            postDataBuilder.AppendFormat("itemcount={0}", modIDs.Length);

            for (var i = 0; i < modIDs.Length; i++)
                postDataBuilder.AppendFormat("&publishedfileids[{0}]={1}", i, modIDs[i]);
            var postData  = postDataBuilder.ToString();
            var byteArray = Encoding.UTF8.GetBytes(postData);

            var request = WebRequest.Create(RequestUri);
            request.Method        = "POST";
            request.ContentType   = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            var dataStream = await request.GetRequestStreamAsync();
            await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using var       response           = await request.GetResponseAsync();
            await using var responseStream     = response.GetResponseStream();
            var             reader             = new StreamReader(responseStream ?? throw new InvalidOperationException());
            var             responseFromServer = await reader.ReadToEndAsync();
            var             apiResponse        = PublishedFileDetails.FromJson(responseFromServer)?.Response;

            if (apiResponse == null)
                return null;

            var resolvedMods = new Dictionary<ulong, ModLocalItem>();

            foreach (var publishedfiledetail in apiResponse.Publishedfiledetails)
            {
                var modItem = new ModLocalItem
                {
                    ModPublishedId = publishedfiledetail.Publishedfileid, ModTitle = publishedfiledetail.Title, ModDescription = publishedfiledetail.Description, ModThumbnail = await GetModThumbnail(publishedfiledetail.PreviewUrl)
                };

                if (!resolvedMods.ContainsKey(publishedfiledetail.Publishedfileid))
                    resolvedMods.Add(publishedfiledetail.Publishedfileid, modItem);
            }

            return resolvedMods;
        }
    }
}
