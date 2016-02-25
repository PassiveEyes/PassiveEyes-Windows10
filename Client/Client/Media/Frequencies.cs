namespace Client.Media
{
    /// <summary>
    /// Frequencies of events, such as uploads or analysis.
    /// </summary>
    public static class Frequencies
    {
        /// <summary>
        /// Analysis frequencies for photos.
        /// </summary>
        public static class Analysis
        {
            /// <summary>
            /// Photo analysis frequency in calm periods (as often as possible).
            /// </summary>
            public const int Calm = 1;

            /// <summary>
            /// Photo analysis frequency in active periods (once a second).
            /// </summary>
            public const int Active = 1000;
        }

        /// <summary>
        /// Upload frequencies for photos.
        /// </summary>
        public static class Uploads
        {
            /// <summary>
            /// Photo upload frequency in calm periods (once an hour).
            /// </summary>
            public const int Calm = 3600000;

            /// <summary>
            /// Photo upload frequency in active periods (once a second).
            /// </summary>
            public const int Active = 1000;
        }
    }
}
