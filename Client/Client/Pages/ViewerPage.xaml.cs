using Client.Media;
using Client.Models;
using Client.OneDrive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Client.Pages
{
    public sealed partial class ViewerPage : Page
    {
        /// <summary>
        /// A reference to the app's view model for data binding.
        /// </summary>
        public ViewModel ViewModel => (Application.Current as App).ViewModel;

        /// <summary>
        /// Input receivers for connected photo devices.
        /// </summary>
        private CameraReceiver[] CameraReceivers;

        private List<CaptureElement> testElements = new List<CaptureElement>();

        public ViewerPage()
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
            await this.InitializeMainPreview();
        }

        private void FeedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedFeed = FeedList.SelectedItem as FeedModel;
        }

        /// <summary>
        /// Initializes camera devices and their corresponding receivers.
        /// </summary>
        private async Task InitializeCameras()
        {
            var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            this.CameraReceivers = cameraDevices
                .Select(cameraDevice => new CameraReceiver(cameraDevice, this.OnFiredUpload))
                .ToArray();

            this.ViewModel.AvailableFeeds.Clear();

            for (int i = 0; i < this.CameraReceivers.Length; i += 1)
            {
                var receiver = this.CameraReceivers[i];
                await receiver.Initialize();

                this.ViewModel.AvailableFeeds.Add(
                    new FeedModel
                    {
                        Name = $"Camera {i}",
                        MediaCapture = receiver.MediaCapture
                    });
            }

            this.ViewModel.SelectedFeed = this.ViewModel.AvailableFeeds.FirstOrDefault();
        }

        /// <summary>
        /// Initializes the main preview's capture source.
        /// </summary>
        /// <todo>Respect the user's selected index.</todo>
        public async Task InitializeMainPreview()
        {
            var mediaCapture = this.ViewModel.SelectedFeed.MediaCapture;

            this.PreviewControl.Source = mediaCapture;

            await mediaCapture.StartPreviewAsync();
        }

        /// <summary>
        /// Responds to a receiver indicating an upload should occur.
        /// </summary>
        /// <param name="mediaCapture">A image capture source.</param>
        /// <todo>Move this logic into a helper class.</todo>
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
