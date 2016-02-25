namespace Client.Pages
{
    using Media;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Media.Capture;
    using Windows.System.Display;
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
        /// Initializes 
        /// </summary>
        /// <param name="index"></param>
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
        /// <returns></returns>
        private async Task OnFiredUpload(MediaCapture mediaCapture)
        {
            Debug.WriteLine("Uploading!");
            // ...
        }
    }
}
