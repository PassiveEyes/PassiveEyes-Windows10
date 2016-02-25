namespace Client.OneDrive
{
    using Microsoft.OneDrive.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;

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
        public PieceOfCrap()
        {
            this.Client = new HttpRestClient
            {
                BasePath = "https://api.onedrive.com/v1.0/",
                DefaultHeaders = new Dictionary<string, string>
                {
                    { "Authorization", "bearer EwB4Aq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAfPjLQItUMZO4nSlTuxXyAozhSm13rFUp5Gz6gZy8ZajUibQ70uiEM9qrpcYk31H0/RSpJZUnTHmLlXOxAGg5n1qKZj48sGegDzrtMayrv0W2KOmvvzjnHK4VxHhvSAJzv6ZMZ9fXu9s29BjgP5keOS66Si69JoLNHT6ADoBA9+3HwFVXccSoDEdGAbRxX8G5dXHjDl59x4oyI1UiqO16NgXh6x7fQcmfVeVMbHnyicCYzNPMY3NEzuFHa37S5KKk39dY1kXRhIEBbP8IhXECJAv5kQpfNppp4VwmRMh+Vi8ABBJ93n7ljaNSTqCz4uWp5ncK3O0RjWxDzYGRSkXcVsDZgAACPLI7V2xpkHiSAFd2Ai3xyWWx98pz0SSnZb+lHKZ/Foq69IhSsbUnMGkRorxuFKRXxwLo5qOtVT+5wl34+WfMkFPQu7ZZbfpbQPYLx47fOJJD9GgLdmxeVQ7CgFxgmsQfua3r9Qo1bHiPTv49kThtSmRJ9ONSC7JPtDcjQ11wdrWjEkbRIY8WzN2fH3VPVFA+qHbgzegPSKNmLZ5PX/s4Xfxbe4pdNt7k3BTsL7YTrt9XqrUrZ9K5qwmtEArQZZpe0Tji15iTLJKSZ0pwvUO/Wm1hH1mEWcxKNLgPcG23TixtfNt753//7yu135JJhlwlfhZvXGSKQGt1XkEG8mSFq/f+Xy0LH6NDtnzcQRPUyfYo3xq5aZqIJOyfCP99eN6DcLVY5np4y9QqmvPyZu3u23vMtMH75iQ0NfikuruSLnTNHL6wP58JTx0Wjh/DscMAB5jXQE=" }
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
        public static T RunAction<T>(Func<T> action)
        {
            try
            {
                return action();
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
        public async Task<Drive> GetDrive() => await this.Client.SendDeserialized<Drive>("drive");

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
