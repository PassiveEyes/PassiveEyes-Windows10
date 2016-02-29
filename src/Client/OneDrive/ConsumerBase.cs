namespace PassiveEyes.SDK.OneDrive
{
    /// <summary>
    /// An abstract PasiveEyes consumer of the OneDrive API.
    /// </summary>
    public abstract class ConsumerBase
    {
        /// <summary>
        /// Utility to interact with the OneDrive API.
        /// </summary>
        protected OneDriveClient Client { get; set; }

        /// <summary>
        /// Generator for OneDrive API filtering.
        /// </summary>
        protected FilterGenerator FilterGenerator { get; } = new FilterGenerator();

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
