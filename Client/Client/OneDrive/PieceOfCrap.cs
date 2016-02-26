namespace Client.OneDrive
{
    using Microsoft.OneDrive.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;

    /// <summary>
    /// A small wrapper around the OneDrive API. Ugh.
    /// </summary>
    public class PieceOfCrap
    {
        /// <summary>
        /// The internal <see cref="HttpRestClient"/> for requests.
        /// </summary>
        private HttpRestClient Client;

        /// <summary>
        /// Initializes a new instance of the Client class.
        /// </summary>
        public PieceOfCrap(string accessToken)
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
        /// Runs an action that may require authorization.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="action">A task to run.</param>
        /// <returns>The result of action.</returns>
        /// <remarks>This is a utility to help with debugging auth failures.</remarks>
        public static T RunAction<T>(Func<PieceOfCrap, T> action)
        {
            try
            {
                return action(((App)Application.Current).Crap);
            }
            catch (UnauthenticatedException error)
            {
                Debug.WriteLine($"Error: {error.Message}");
                throw;
            }
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
