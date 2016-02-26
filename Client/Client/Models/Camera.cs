namespace Client.Models
{
    using Media;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Media.Capture;

    /// <summary>
    /// A representation of a webcam or camera.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The name of the camera.
        /// </summary>
        public string Name { get; private set; }

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
        /// Handles the receiver signaling for an upload.
        /// </summary>
        /// <param name="mediaCapture">The receiver's media.</param>
        private Task HandleUpload(MediaCapture mediaCapture)
        {
            // System.Diagnostics.Debugger.Break();
            return null;
        }

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
    }
}
