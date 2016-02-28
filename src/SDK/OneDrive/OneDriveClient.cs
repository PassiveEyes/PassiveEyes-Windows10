using Microsoft.OneDrive.Sdk;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PassiveEyes.SDK.OneDrive
{
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
        /// Retrieves the user's <see cref="Drive"/>.
        /// </summary>
        /// <returns>The user's <see cref="Drive"/>.</returns>
        public async Task<Drive> GetDrive() => await this.Client.SendDeserializedGetRequest<Drive>("drive");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemPath"></param>
        /// <returns></returns>
        public async Task<T> GetItemChildren<T>(string itemPath)
            => await this.Client.SendDeserializedGetRequest<T>($"drive/root:/{itemPath}:/children");

        /// <summary>
        /// Uploads an item to the drive.
        /// </summary>
        /// <param name="folderPath">The folder path of the item.</param>
        /// <param name="fileName">The file name of the item.</param>
        /// <param name="contents">The contents of the item.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutItem(string folderPath, string fileName, string contents)
        {
            return await this.Client.SendPutRequest(
                this.GenerateOneDrivePath(folderPath, fileName, "content"),
                contents);
        }

        /// <summary>
        /// Uploads an item to the drive.
        /// </summary>
        /// <param name="folderPath">The folder path of the item.</param>
        /// <param name="fileName">The file name of the item.</param>
        /// <param name="contents">The contents of the item.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutItem(string folderPath, string fileName, HttpContent contents)
        {
            return await this.Client.SendPutRequest(
                this.GenerateOneDrivePath(folderPath, fileName, "content"),
                contents);
        }

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
