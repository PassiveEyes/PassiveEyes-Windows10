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
        public PieceOfCrap()
        {
            this.Client = new HttpRestClient
            {
                BasePath = "https://api.onedrive.com/v1.0/",
                DefaultHeaders = new Dictionary<string, string>
                {
                    { "Authorization", "bearer EwB4Aq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAVe3BdFCp+z9YFvCo3fNqE+EZlqBv9EFRH2jnGmd0mcmSHdR+hsxFugrzXLBCxo45JMr2b6yqFYTSex9uxUXBP284EUFcaeg3B9VyV0caYfK+BXBxyqj24GUtoaQoG6lMVhkM5z02V9B/p1yuR3fbVfTIdy6c36Pfx/g+D8BX5Xy++VHzKNG2zLpM6iPI1Qt6m08zS1Jjmy5NGq6iau7eRX/NfZ+ix7kkSnfG/N4c2Md57URv/nzj+FnLihP6gK770wf312kuuSU/EcYfVIQmc2lIq6JUqNLQurkpO2VZoOMJNqUv0xB+LutkAZv1tD+Mr+7Asv5Dat6+JBopOgoDG4DZgAACPXnYGGS9M4GSAHGZQr92Ilr2BYO/OvUao65GzmlMPT9vOM+bxCIue5FxyAYEP8CKisMyqHPDKR6eZ/Ylq0VxgfX43K5ZOKoSzTKUTv1XJTvZGZQ5/7GIED397CXFNWtD7H1dr4lqvR474nGTtD4zRlrwnALhFXKTew+4lwz0VCHetbR2CR5bwM16eoaH32rfyD4TQ+evHBkmCNYtNJCcSUZq+ugDhdK26MtJtXdh0zBcD/QeYpZcqeD3VtyPzk4Z+drS76pQFXH+z2tAuQvrcvkfGzAiHn/Wtz+aLHQqLkoxrn/5r3au4NGgeZRUXWDm2pRjLbpR6nwY0r9al3iTT1ph/azlzdFpn0dLpGqKcUqb3D0hKmYefNr1JDBcG30iaKYGBOK+HW5d6BCQVdYGOrwfZTfAP49fMzONumU7BjLqeX5rL0+Ogwwg6FQrhzcb2ICXQE=" }
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
