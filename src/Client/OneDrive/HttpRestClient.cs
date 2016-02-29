using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PassiveEyes.SDK
{
    /// <summary>
    /// An <see cref="Exception"/> for unauthorized errors.
    /// </summary>
    internal class UnauthenticatedException : Exception
    {
        public UnauthenticatedException() : base("Do you need to get a new authorization token?") { }
    }

    /// <summary>
    /// A base class for sending HTTP REST requests.
    /// </summary>
    public class HttpRestClient
    {
        /// <summary>
        /// The base path for all requests.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Internal default headers to attach to all requests.
        /// </summary>
        private Dictionary<string, string> defaultHeaders;

        /// <summary>
        /// Default headers to attach to all requests.
        /// </summary>
        public Dictionary<string, string> DefaultHeaders
        {
            get
            {
                return this.defaultHeaders;
            }
            set
            {
                this.defaultHeaders = value;
                this.Client.DefaultRequestHeaders.Clear();

                foreach (var header in value)
                {
                    this.Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// The internal <see cref="HttpClient"/> for requests.
        /// </summary>
        private HttpClient Client = new HttpClient();

        /// <summary>
        /// Sends a general GET request and parses the response content.
        /// </summary>
        /// <typeparam name="T">The class type of the content.</typeparam>
        /// <param name="path">A path after the base path.</param>
        /// <param name="parameters">Additional key-value parameters for the request.</param>
        /// <returns>The response, deserialized to type T.</returns>
        public async Task<T> SendDeserializedGetRequest<T>(string path = "", IDictionary<string, string> parameters = null)
        {
            var request = await this.SendGetRequest(path, parameters);

            if (request.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthenticatedException();
            }

            return JsonConvert.DeserializeObject<T>(await request.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Sends a general GET request.
        /// </summary>
        /// <param name="path">A path after the base path.</param>
        /// <param name="parameters">Additional key-value parameters for the request.</param>
        /// <returns>The response object.</returns>
        public async Task<HttpResponseMessage> SendGetRequest(string path = "", IDictionary<string, string> parameters = null)
        {
            return await this.Client.GetAsync(this.GenerateRequestUri(path));
        }

        /// <summary>
        /// Sends a general PUT request.
        /// </summary>
        /// <param name="path">A path after the base path.</param>
        /// <param name="contents">Contents of the file to create.</param>
        /// <returns>The response object.</returns>
        public async Task<HttpResponseMessage> SendPutRequest(string path, string contents = "")
            => await this.SendPutRequest(path, new StringContent(contents));

        /// <summary>
        /// Sends a general PUT request.
        /// </summary>
        /// <param name="path">A path after the base path.</param>
        /// <param name="contents">Contents of the file to create.</param>
        /// <returns>The response object.</returns>
        public async Task<HttpResponseMessage> SendPutRequest(string path, HttpContent contents)
        {
            return await this.Client.PutAsync(
                this.GenerateRequestUri(path),
                contents);
        }

        /// <summary>
        /// Generates a complete request URI given a path and any parameters.
        /// </summary>
        /// <param name="path">A path to append to base path.</param>
        /// <param name="parameters">Any additional parameters.</param>
        /// <returns>A complete request URI.</returns>
        private string GenerateRequestUri(string path, Dictionary<string, string> parameters = null)
        {
            var requestUri = $"{this.BasePath}{path}";

            if (parameters != null)
            {
                requestUri += "?" + string.Join("&", parameters.Select((key, value) => $"{key}={value}"));
            }

            return requestUri;
        }
    }
}
