namespace Client.OneDrive
{
    using Microsoft.OneDrive.Sdk;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// A small wrapper around the OneDrive API. Ugh.
    /// </summary>
    public class PieceOfCrap
    {
        /// <summary>
        /// An hour-long bearer token used for authorization.
        /// </summary>
        private static readonly string Authorization = "bearer EwB4Aq1DBAAUGCCXc8wU/zFu9QnLdZXy+YnElFkAAfgJObXWCIrADyOPj+IyTXT8plBsQYTdjGCza/vFjSPAyXAxtrHHdpwZ8ktrA+wv5mpWlpuHYdkpXNU4JjqYX82p7BT1Ml2azRzXxfFqt5VtcReAJyseniYhzHcZWzfV6BALuFVtV/ajB/rZOW5G2SStx9X9Eo2r9jSYIt98lEAlDslyHXRs6Q10wLcPHX0mCvQ7foRT9uTVD/7n8tOEvRHBvoehBb83hW/71iUY/N6ryqKxEkSJHuVIlPmp1hmEcdxH/zJZTZh7DAu/igJ6sRHrMAhrsCqMpOSbcGzd2xGx3yMJYUIajdsIHyGn9fEhSS/ysE9unCCkoDNES3aq1msDZgAACEzEfG+0QiqQSAF18+2xajVo418ztlr1xGDoZEXMXDzprSvb/hsJgT/yNujjBVM6Oc7/Cfsl489WX9CI+OwzMq/NYYa6+rZWXWJvikT9nvvDzmRr1H/1Dn5r/GuJ+S2wTjrjhMcTwYjDweDRggIS27JHvgCfIN9F6GCweyexiJFLduAoXjOvlKPv21MNSXzPtRhYfcZbh3iRhcmEwbG1MDEdpJdQAv34dXHunze4roRgu8HP/e+nlW5bfZ3nPPrqW9unGsV9E1zgrg+VHrhf0MT4BwNsoi54MH8pycZ8FuPMPyQ8GzR0Lx9MZLtzuBZl6pf815xWdOYcqG+d/iv2YQbST29dIkb2zlYMyjA6XaMDZJ60MdfWuql7mGdn5OiVBIGa3rdc7PtHwbY9PEMITTJERFQA3t0ezDBIVkoE74XdQ5eBDnA7OqmqZ0qF/THRbAqYXQE=";

        /// <summary>
        /// The internal <see cref="HttpClient"/> for requests.
        /// </summary>
        private HttpClient Client;

        /// <summary>
        /// Initializes a new instance of the Client class.
        /// </summary>
        public PieceOfCrap()
        {
            this.Client = new HttpClient();
            this.Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        }

        /// <summary>
        /// Retrieves the user's <see cref="Drive"/>.
        /// </summary>
        /// <returns>The user's <see cref="Drive"/>.</returns>
        public async Task<Drive> GetDrive() => await this.SendParsedGetRequest<Drive>("drive");

        /// <summary>
        /// Sends a general GET request and parses the response content.
        /// </summary>
        /// <typeparam name="T">The class type of the content.</typeparam>
        /// <param name="path">The path within OneDrive.</param>
        /// <param name="parameters">Additional key-value parameters for the request.</param>
        /// <returns>The response, parsed to type T.</returns>
        private async Task<T> SendParsedGetRequest<T>(string path, Dictionary<string, string> parameters = null)
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
        /// <param name="path">The path within OneDrive.</param>
        /// <param name="parameters">Additional key-value parameters for the request.</param>
        /// <returns>The response object.</returns>
        private async Task<HttpResponseMessage> SendGetRequest(string path, Dictionary<string, string> parameters = null)
        {
            var requestUri = $"https://api.onedrive.com/v1.0/{path}";

            if (parameters != null)
            {
                requestUri += "?" + string.Join(
                    "&",
                    parameters.Select((key, value) => $"{key}={value}"));

                requestUri = requestUri.Substring(0, requestUri.Length - 1);
            }

            return await this.Client.GetAsync(requestUri);
        }
    }
}
