using Microsoft.OneDrive.Sdk;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PassiveEyes.SDK.OneDrive.Storage;

namespace PassiveEyes.SDK.OneDrive
{
    /// <summary>
    /// Utility to interact with the OneDrive API.
    /// </summary>
    public class OneDriveClient
    {
        /// <summary>
        /// The internal <see cref="HttpRestClient"/> for requests.
        /// </summary>
        private HttpRestClient Client;

        /// <summary>
        /// Initializes a new instance of the OneDriveClient class.
        /// </summary>
        /// <param name="accessToken">An authorization access token.</param>
        public OneDriveClient(string accessToken)
        {
            this.Client = new HttpRestClient
            {
                BasePath = "https://api.onedrive.com/v1.0/",
                DefaultHeaders = new Dictionary<string, string>
                {
                    { "Authorization", "bearer " + accessToken }
                }
            };
        }

        /// <summary>
        /// Retrives the children of an item path.
        /// </summary>
        /// <param name="itemPath">A OneDrive path to retrieve children from.</param>
        /// <param name="parameters">Additional key-value parameters for the request.</param>
        /// <returns></returns>
        public async Task<Item[]> GetItemChildren(string itemPath, IDictionary<string, string> parameters = null)
            => (await this.Client.SendDeserializedGetRequest<Children>($"drive/root:/{itemPath}:/children", parameters)).Value;

        /// <summary>
        /// Retrives the children of an item path.
        /// </summary>
        /// <param name="itemPath">A OneDrive path to retrieve children from.</param>
        /// <param name="filter">Stringified filter to search on.</param>
        /// <returns></returns>
        public async Task<Item[]> GetItemChildren(string itemPath, string filter)
            => (await
                this.Client.SendDeserializedGetRequest<Children>(
                    $"drive/root:/{itemPath}:/children",
                    new Dictionary<string, string> { { "filter", filter } }))
                .Value;

        /// <summary>
        /// Uploads an item to the drive.
        /// </summary>
        /// <param name="folderPath">The folder path of the item.</param>
        /// <param name="fileName">The file name of the item.</param>
        /// <param name="contents">The contents of the item.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutItem(string folderPath, string fileName, string contents)
            => await this.Client.SendPutRequest(
                this.GenerateOneDrivePath(folderPath, fileName, "content"),
                contents);

        /// <summary>
        /// Uploads an item to the drive.
        /// </summary>
        /// <param name="folderPath">The folder path of the item.</param>
        /// <param name="fileName">The file name of the item.</param>
        /// <param name="contents">The contents of the item.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutItem(string folderPath, string fileName, HttpContent contents)
            => await this.Client.SendPutRequest(
                this.GenerateOneDrivePath(folderPath, fileName, "content"),
                contents);

        /// <summary>
        /// Retrieves item contents for an item.
        /// </summary>
        /// <param name="folderPath">A folder path within the drive.</param>
        /// <param name="fileName">A file name within the drive.</param>
        /// <returns>The item's contents.</returns>
        public async Task<Stream> GetItemContents(string folderPath, string fileName)
        {
            var response = await this.Client.SendGetRequest(
                this.GenerateOneDrivePath(folderPath, fileName, "content"));

            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Generates an API path for after the root path.
        /// </summary>
        /// <param name="folderPath">A folder path within the drive.</param>
        /// <param name="fileName">A file name within the drive.</param>
        /// <param name="extra">Any extra characters after the path.</param>
        /// <returns></returns>
        private string GenerateOneDrivePath(string folderPath, string fileName, string extra = "")
            => $"drive/root:/{folderPath}/{fileName}:/{extra}";
    }
}
