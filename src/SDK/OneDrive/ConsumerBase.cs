namespace PassiveEyes.SDK.OneDrive
{
    public class ConsumerBase
    {
        protected OneDriveClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerBase"/> class.
        /// </summary>
        /// <param name="accessToken">An authorization access token.</param>
        public ConsumerBase(string accessToken = "")
        {
            this.Client = new OneDriveClient(accessToken);
        }
    }
}
