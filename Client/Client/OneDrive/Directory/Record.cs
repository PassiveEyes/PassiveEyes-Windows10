namespace Client.OneDrive.Directory
{
    using Microsoft.OneDrive.Sdk;
    using System;

    /// <summary>
    /// An individual picture retrieved from OneDrive storage.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// The represented OneDrive item.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// The name of the webcam that took this photo.
        /// </summary>
        public string Webcam { get; set; }

        /// <summary>
        /// When the item was created.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="item">The record's represented OneDrive item.</param>
        /// <returns>The item's equivalent representation.</returns>
        internal static Record FromChild(Item item)
        {
            return new Record
            {
                Item = item,
                Webcam = ExtractWebcamNameFromFileName(item.Name),
                Timestamp = item.CreatedDateTime.Value
            };
        }

        /// <summary>
        /// Extracts a webcam name from an image file name.
        /// </summary>
        /// <param name="fileName">An image file name.</param>
        /// <returns>The name of the webcam that took the image.</returns>
        private static string ExtractWebcamNameFromFileName(string fileName)
        {
            var split = fileName.Split('-');

            return split[split.Length - 2];
        }
    }
}
