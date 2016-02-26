namespace Client.Models
{
    using Media;
    using OneDrive;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Media.Capture;
    using Windows.UI.Xaml;

    /// <summary>
    /// A representation of a webcam or camera.
    /// </summary>
    public class Camera : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the camera feed.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }
        private string name;

        /// <summary>
        /// Handler for the camera input.
        /// </summary>
        public CameraReceiver Receiver { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="name">The name of the camera.</param>
        /// <param name="cameraDevice">Information on the streaming camera device.</param>
        /// <param name="uploadHandler">An external handler for uploading a photo.</param>
        public Camera(string name, DeviceInformation cameraDevice)
        {
            this.Name = name;
            this.Receiver = new CameraReceiver(cameraDevice, this.HandleUpload);
        }

        /// <summary>
        /// Responds to a receiver indicating an upload should occur.
        /// </summary>
        /// <param name="mediaCapture">An image capture source.</param>
        private async Task HandleUpload(MediaCapture mediaCapture)
        {
            var folderName = ((App)Application.Current).StorageFolderPath;
            var fileName = this.GenerateFileName();

            using (var fileStore = await TemporaryCaptureFileStore.Create(fileName, mediaCapture))
            {
                await PieceOfCrap.RunAction(async (PieceOfCrap crap) =>
                {
                    await crap.PutItem(folderName, fileName, new StreamContent(fileStore.OutputStream));
                });

                await fileStore.DisposeAsync();
            }
        }

        /// <summary>
        /// Generates a file name for a new file based on the current time and name.
        /// </summary>
        /// <returns>A file name for a new file.</returns>
        private string GenerateFileName() => $"PassiveEyes-Snapshot-{DateTime.Now.ToFileTimeUtc()}-{this.Name}-{(this.Receiver.Active ? 1 : 0)}.jpg";

        /// <summary>
        /// Converts available cameras into <see cref="Camera"/> instances.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<Camera>> CollectCameras()
        {
            return
                (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                    .Select((device, i) => new Camera($"Camera {i}", device));
        }

        #region Property Changed Events
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
