namespace Client.OneDrive.Directory
{
    using Microsoft.OneDrive.Sdk;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An individual picture retrieved from OneDrive storage.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Whether the record was active at the time.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The represented OneDrive item.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// When the item was created.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// The name of the webcam that took this photo.
        /// </summary>
        public string Webcam { get; set; }

        /// <summary>
        /// Generates a <see cref="BitmapImage"/> by downloading the stored item.
        /// </summary>
        /// <returns>A newly created <see cref="BitmapImage"/>.</returns>
        public async Task<BitmapImage> GenerateBitmap()
        {
            return await PieceOfCrap.RunAction(
                async (PieceOfCrap crap) =>
                {
                    var inputStream = await crap.GetItemContents("PassiveEyes", this.Item.Name);

                    using (var outputStream = new MemoryStream())
                    {
                        await inputStream.CopyToAsync(outputStream);
                        outputStream.Position = 0;

                        var bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(outputStream.AsRandomAccessStream());
                        return bitmap;
                    }
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="item">The record's represented OneDrive item.</param>
        /// <returns>The item's equivalent representation.</returns>
        internal static Record FromChild(Item item)
        {
            var nameSplit = item.Name.Split('-');
            
            return new Record
            {
                Item = item,
                Active = int.Parse(nameSplit[nameSplit.Length - 1].Split('.')[0]) == 1,
                Timestamp = item.CreatedDateTime.Value,
                Webcam = nameSplit[nameSplit.Length - 2]
            };
        }
    }
}
