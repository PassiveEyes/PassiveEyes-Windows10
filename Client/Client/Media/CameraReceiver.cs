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
        private Timer AnalysisTimer;

        /// <summary>
        /// How often uploads should occur, in milliseconds.
        /// </summary>
        private int UploadFrequency;

        /// <summary>
        /// How often analyses should occur, in milliseconds.
        /// </summary>
        private int AnalysisFrequency;

        /// <summary>
        /// How many photos should be immediately uploaded due to an active state.
        /// </summary>
        private int RemainingActivePhotos;

        /// <summary>
        /// When the last upload was triggered.
        /// </summary>
        private DateTime LastUploaded = DateTime.Now;

        /// <summary>
        /// External handler for uploading a photo.
        /// </summary>
        private UploadResponder UploadHandler;

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

            this.ScheduleNextAnalysis();
        }

        /// <summary>
        /// Schedules the next analysis event.
        /// </summary>
        /// <param name="millisecondsDelayed">How many seconds this was delayed.</param>
        private void ScheduleNextAnalysis(int millisecondsDelayed = 0)
        {
            this.AnalysisTimer = new Timer(
                this.FireAnalysisTimer, 
                null,
                Math.Max(0, this.AnalysisFrequency - millisecondsDelayed),
                Timeout.Infinite);
        }

        /// <summary>
        /// Triggers an analysis event, checking if the state has changed significantly.
        /// </summary>
        /// <param name="_">The (ignored) timer state.</param>
        private async void FireAnalysisTimer(object _)
        {
            var timeFired = DateTime.Now;

            this.AnalysisTimer.Dispose();

            if (this.CaptureProcessor.CheckForSignificantImageChanges(await this.GetPixelDataFromCapture()))
            {
                this.RemainingActivePhotos = 3;
                this.AnalysisFrequency = Frequencies.Analysis.Active;
                this.UploadFrequency = Frequencies.Uploads.Active;
            }
            else
            {
                this.AnalysisFrequency = Frequencies.Analysis.Calm;
                this.UploadFrequency = Frequencies.Uploads.Calm;
            }

            await this.UploadIfNecessary();

            this.ScheduleNextAnalysis(Math.Max(0, (DateTime.Now - timeFired).Milliseconds));
        }

        /// <summary>
        /// Calls the upload handler if an active event recently triggered or enough time
        /// has passed since the last upload.
        /// </summary>
        private async Task UploadIfNecessary()
        {
            var timeDelta = (DateTime.Now - this.LastUploaded);
            
            if (this.RemainingActivePhotos > 0)
            {
                this.RemainingActivePhotos -= 1;
            }
            else if (timeDelta.TotalMilliseconds < this.UploadFrequency)
            {
                return;
            }

            await this.UploadHandler(this.MediaCapture);
            this.LastUploaded = DateTime.Now;
        }

        /// <summary>
        /// Retrieves the raw byte data from the media capture.
        /// </summary>
        /// <returns>Raw byte data from the media capture.</returns>
        private async Task<byte[]> GetPixelDataFromCapture()
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                await this.MediaCapture.CapturePhotoToStreamAsync(
                    ImageEncodingProperties.CreateBmp(),
                    stream);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var dataProvider = await decoder.GetPixelDataAsync();

                return dataProvider.DetachPixelData();
            }
        }
    }
}
