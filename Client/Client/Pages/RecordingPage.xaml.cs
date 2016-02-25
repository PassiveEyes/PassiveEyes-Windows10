namespace Client.Pages
{
    using Media;
    using Microsoft.OneDrive.Sdk;
    using OneDrive;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Foundation;
    using Windows.Graphics.Imaging;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage;
    using Windows.Storage.FileProperties;
    using Windows.Storage.Streams;
    using Windows.System.Display;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// The webcam client's recording page.
    /// </summary>
    public sealed partial class RecordingPage : Page
    {
        /// <summary>
        /// The display request to keep the computer out of sleep mode.
        /// </summary>
        private readonly DisplayRequest DisplayRequest = new DisplayRequest();

        /// <summary>
        /// Input receivers for connected photo devices.
        /// </summary>
        private CameraReceiver[] CameraReceivers;

        /// <summary>
        /// The index of the camera being sent to the UI preview control.
        /// </summary>
        private int PreviewingIndex = -1;

        /// <summary>
        /// Initializes a new instance of the RecordingPage class.
        /// </summary>
        public RecordingPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source
        /// of a parent Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.InitializeCameras();
            await this.InitializePreview(0);
        }

        /// <summary>
        /// Initializes camera devices and their corresponding receivers.
        /// </summary>
        private async Task InitializeCameras()
        {
            var cameraDevices = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture));

            this.CameraReceivers = cameraDevices
                .Select(cameraDevice => new CameraReceiver(cameraDevice, this.OnFiredUpload))
                .ToArray();

            foreach (var receiever in this.CameraReceivers)
            {
                await receiever.Initialize();
            }
        }

        /// <summary>
        /// Initializes the visual preview of the content stream.
        /// </summary>
        /// <param name="index">The index of the camera input to preview.</param>
        private async Task InitializePreview(int index)
        {
            if (this.PreviewingIndex == index)
            {
                return;
            }

            if (this.PreviewingIndex != -1)
            {
                this.PreviewControl.Source = null;
                await this.CameraReceivers[this.PreviewingIndex].MediaCapture.StopPreviewAsync();
            }

            var mediaCapture = this.CameraReceivers[index].MediaCapture;

            this.PreviewControl.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();

            this.PreviewingIndex = index;
        }

        /// <summary>
        /// Responds to a receiver indicating an upload should occur.
        /// </summary>
        /// <param name="mediaCapture"></param>
        private async Task OnFiredUpload(MediaCapture mediaCapture)
        {
            var fileName = $"Upload-{DateTime.Now.ToFileTimeUtc()}.jpg";

            using (var inputStream = new InMemoryRandomAccessStream())
            {
                await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), inputStream);
                var file = await ReencodeAndSavePhotoAsync(fileName, inputStream);

                var crap = ((App)Application.Current).Crap;
                var folderName = ((App)Application.Current).StorageFolderPath;
                var testfile = await StorageFile.GetFileFromPathAsync(file.Path);
                var writeStream = await testfile.OpenStreamForReadAsync();

                await PieceOfCrap.RunAction(async () =>
                {
                    await crap.PutItem(folderName, fileName, new StreamContent(writeStream));
                });
            }
        }

        /// <summary>
        /// Applies the given orientation to a photo stream and saves it as a StorageFile.
        /// </summary>
        /// <param name="fileName">Name the file will be stored under.</param>
        /// <param name="stream">The photo stream.</param>
        /// <returns>A newly created image file.</returns>
        private static async Task<StorageFile> ReencodeAndSavePhotoAsync(string fileName, IRandomAccessStream stream)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

                using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                    var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) } };

                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                }

                return file;
            }
        }
    }
}
