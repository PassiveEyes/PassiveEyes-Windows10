namespace Client.OneDrive
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Graphics.Imaging;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage;
    using Windows.Storage.FileProperties;
    using Windows.Storage.Streams;

    /// <summary>
    /// A temporary container for transcoding an image into a stream.
    /// </summary>
    /// <remarks>Not being used currently, due to threading issues.</remarks>
    public class TemporaryCaptureFileStore : IDisposable
    {
        /// <summary>
        /// Default properties to pass to encoded images.
        /// </summary>
        private static BitmapPropertySet PhotoProperties = new BitmapPropertySet
        {
            {
                "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16)
            }
        };

        /// <summary>
        /// An output stream from the saved jpeg file.
        /// </summary>
        public Stream OutputStream { get; private set; }

        /// <summary>
        /// An in-memory store for a jpeg file.
        /// </summary>
        public StorageFile StorageFile { get; private set; }

        /// <summary>
        /// Disposes the files tore.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Disposes file resources asynchronously.
        /// </summary>
        public async Task DisposeAsync()
        {
            await this.StorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        /// <summary>
        /// Asynchronously initializes a new instance of the <see cref="TemporaryCaptureFileStore"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="mediaCapture">An image capture source.</param>
        /// <param name="inputStream">An input stream for the file.</param>
        /// <returns>A new file store, with a stream from a stored file.</returns>
        public static async Task<TemporaryCaptureFileStore> Create(string fileName, MediaCapture mediaCapture)
        {
            var inputStream = new InMemoryRandomAccessStream();
            await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), inputStream);
            
            var storageFile = await ReencodeAndSavePhotoAsync(fileName, inputStream);
            var encodedFile = await StorageFile.GetFileFromPathAsync(storageFile.Path);
            var outputStream = await encodedFile.OpenStreamForReadAsync();

            return new TemporaryCaptureFileStore
            {
                StorageFile = storageFile,
                OutputStream = outputStream
            };
        }

        /// <summary>
        /// Applies the given orientation to a photo stream and saves it as a StorageFile.
        /// </summary>
        /// <param name="fileName">Name the file will be stored under.</param>
        /// <param name="inputStream">The photo stream.</param>
        /// <returns>A newly created image file.</returns>
        private static async Task<StorageFile> ReencodeAndSavePhotoAsync(string fileName, IRandomAccessStream inputStream)
        {
            var decoder = await BitmapDecoder.CreateAsync(inputStream);
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

            using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                await encoder.BitmapProperties.SetPropertiesAsync(PhotoProperties);
                await encoder.FlushAsync();
            }

            return file;
        }
    }
}
