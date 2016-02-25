namespace Client.Media
{
    using Processing;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Graphics.Imaging;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage.Streams;

    /// <summary>
    /// External handler for an upload being requested.
    /// </summary>
    /// <param name="mediaCapture">The media capture whose image is to be uploaded.</param>
    public delegate Task UploadResponder(MediaCapture mediaCapture);

    /// <summary>
    /// Receives input from a camera device, and runs tasks on it at an interval.
    /// </summary>
    public class CameraReceiver
    {
        /// <summary>
        /// Information on the streaming camera device.
        /// </summary>
        public DeviceInformation CameraDevice { get; private set; }

        /// <summary>
        /// Captured input from the streaming camera device.
        /// </summary>
        public MediaCapture MediaCapture { get; private set; }

        /// <summary>
        /// Analysis processor for the streaming camera input.
        /// </summary>
        public AnalysisProcessor CaptureProcessor { get; private set; }

        /// <summary>
        /// Periodically triggers analysis events on the captured input.
        /// </summary>
        public Timer AnalysisTimer { get; private set; }

        /// <summary>
        /// Periodically triggers external upload events on captured input.
        /// </summary>
        public Timer UploadTimer { get; private set; }

        /// <summary>
        /// External handler for uploading a photo.
        /// </summary>
        private UploadResponder UploadHandler;

        /// <summary>
        /// How often analysis events should trigger.
        /// </summary>
        private int AnalysisFrequency = Frequencies.Analysis;

        /// <summary>
        /// How often upload events should trigger.
        /// </summary>
        private int UploadFrequency = Frequencies.Uploads.Calm;

        /// <summary>
        /// Initializes a new instance of the CameraReceiver class.
        /// </summary>
        /// <param name="cameraDevice">Information on the streaming camera device.</param>
        /// <param name="uploadHandler">An external handler for uploading a photo.</param>
        public CameraReceiver(DeviceInformation cameraDevice, UploadResponder uploadHandler)
        {
            this.CameraDevice = cameraDevice;
            this.MediaCapture = new MediaCapture();
            this.CaptureProcessor = new AnalysisProcessor();
            this.UploadHandler = uploadHandler;
        }

        /// <summary>
        /// Initializes the receiver to start capturing, analyzing, and uploading.
        /// </summary>
        public async Task Initialize()
        {
            await this.MediaCapture.InitializeAsync(
                new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = this.CameraDevice.Id
                });

            this.ResetAnalysisTimer();
            this.ResetUploadTimer();
        }

        /// <summary>
        /// Resets the timer for analysis events.
        /// </summary>
        private void ResetAnalysisTimer()
        {
            this.AnalysisTimer = new Timer(this.FireAnalysisTimer, null, this.AnalysisFrequency, this.AnalysisFrequency);
        }

        /// <summary>
        /// Resets the timer for upload events.
        /// </summary>
        private void ResetUploadTimer()
        {
            this.UploadTimer = new Timer(this.FireUploadTimer, null, this.UploadFrequency, this.UploadFrequency);
        }

        /// <summary>
        /// Triggers an analysis event, checking if the state has changed significantly.
        /// </summary>
        /// <param name="_">The (ignored) timer state.</param>
        private async void FireAnalysisTimer(object _)
        {
            this.AnalysisTimer.Dispose();
            var bytes = await this.GetPixelDataFromCapture();

            if (this.CaptureProcessor.CheckForSignificantImageChanges(bytes))
            {
                this.UpdateUploadFrequency(Frequencies.Uploads.Active);
                await this.TriggerUpload();
            }
            else
            {
                this.UpdateUploadFrequency(Frequencies.Uploads.Calm);
                this.ResetUploadTimer();
            }

            this.ResetAnalysisTimer();
        }

        /// <summary>
        /// Triggers an upload event.
        /// </summary>
        /// <param name="_"></param>
        private async void FireUploadTimer(object _) => await this.TriggerUpload();

        /// <summary>
        /// Triggers an upload event.
        /// </summary>
        private async Task TriggerUpload() => await this.UploadHandler(this.MediaCapture);
        
        /// <summary>
        /// Updates the upload frequency.
        /// </summary>
        /// <param name="value">A new value for the frequency.</param>
        private void UpdateUploadFrequency(int value)
        {
            if (this.UploadFrequency == value)
            {
                return;
            }

            this.UploadFrequency = value;
            this.UploadTimer.Change(value, value);
        }

        /// <summary>
        /// Retrieves the raw byte data from the media capture.
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> GetPixelDataFromCapture()
        {
            var stream = new InMemoryRandomAccessStream();
            await this.MediaCapture.CapturePhotoToStreamAsync(
                ImageEncodingProperties.CreateBmp(),
                stream);

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var dataProvider = await decoder.GetPixelDataAsync();

            return dataProvider.DetachPixelData();
        }
    }
}
