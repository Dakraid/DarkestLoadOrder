using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using DarkestLoadOrder.Json.SteamWebAPI;

namespace DarkestLoadOrder.ModUtility
{
    public class ModResolverOnline
    {
        private const string RequestUri = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";

        public static async Task<byte[]> GetModThumbnail(Uri thumbnailUri)
        {
            using var httpClient = new HttpClient();
            var       imageBytes = await httpClient.GetByteArrayAsync(thumbnailUri);

            return imageBytes;
        }

        public static Response GetModInfos(ulong[] modIDs)
        {
            var postDataBuilder = new StringBuilder();
            postDataBuilder.AppendFormat("itemcount={0}", modIDs.Length);

            for (var i = 0; i < modIDs.Length; i++)
            {
                postDataBuilder.AppendFormat("&publishedfileids[{0}]={1}", i, modIDs[i]);
            }
            var postData  = postDataBuilder.ToString();
            var byteArray = Encoding.UTF8.GetBytes(postData);

            var request = WebRequest.Create(RequestUri);
            request.Method        = "POST";
            request.ContentType   = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using var response           = request.GetResponse();
            using var responseStream     = response.GetResponseStream();
            var       reader             = new StreamReader(responseStream ?? throw new InvalidOperationException());
            var       responseFromServer = reader.ReadToEnd();

            return PublishedFileDetails.FromJson(responseFromServer)?.Response;
        }

        public static Response GetModInfo(ulong modId)
        {
            var postData  = $"itemcount=1&publishedfileids[0]={modId}";
            var byteArray = Encoding.UTF8.GetBytes(postData);

            var request = WebRequest.Create(RequestUri);
            request.Method        = "POST";
            request.ContentType   = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using var response           = request.GetResponse();
            using var responseStream     = response.GetResponseStream();
            var       reader             = new StreamReader(responseStream ?? throw new InvalidOperationException());
            var       responseFromServer = reader.ReadToEnd();

            return PublishedFileDetails.FromJson(responseFromServer)?.Response;
        }
    }
}
