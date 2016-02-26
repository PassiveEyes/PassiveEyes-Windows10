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

        /// <summary>
        /// Reacts to the feed list changing the selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="mediaCapture">An image capture source.</param>
        /// <todo>Move this logic into a helper class.</todo>
        private async Task OnFiredUpload(MediaCapture mediaCapture)
        {
            var folderName = ((App)Application.Current).StorageFolderPath;
            var fileName = $"Upload-{DateTime.Now.ToFileTimeUtc()}.jpg";

            using (var inputStream = new InMemoryRandomAccessStream())
            using (var fileStore = await TemporaryCaptureFileStore.Create(fileName, mediaCapture, inputStream))
            {
                await PieceOfCrap.RunAction(async (PieceOfCrap crap) =>
                {
                    await crap.PutItem(folderName, fileName, new StreamContent(fileStore.OutputStream));
                });

                await fileStore.DisposeAsync();
            }
        }
    }
}
