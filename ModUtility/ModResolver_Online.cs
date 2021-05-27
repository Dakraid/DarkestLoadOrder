namespace DarkestLoadOrder.ModUtility
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using Json.SteamWebAPI;

    public class ModResolverOnline
    {
        private const string RequestUri = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";

        public Response GetModInfo(ulong modID, bool showDebug = false)
        {
            // Create a request using a URL that can receive a post.
            var request = WebRequest.Create(RequestUri);
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            var postData  = $"itemcount=1&publishedfileids[0]={modID}";
            var byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            var dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            var response = request.GetResponse();
            // Display the status.
            // MessageBox.Show(((HttpWebResponse) response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using var responseStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader(responseStream ?? throw new InvalidOperationException());
            // Read the content.
            var responseFromServer = reader.ReadToEnd();
            // Display the content.
            // MessageBox.Show(responseFromServer);

            // Close the response.
            response.Close();

            var responseData = PublishedFileDetails.FromJson(responseFromServer);

            return responseData?.Response;
        }
    }
}
