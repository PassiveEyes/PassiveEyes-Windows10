using System.Threading.Tasks;
using System.Linq;
using Microsoft.OneDrive.Sdk;
using System;

namespace PassiveEyes.SDK.OneDrive
{
    /// <summary>
    /// Reads metadata and files from the user's OneDrive.
    /// </summary>
    /// <todo>Verify and use filter parameters everywhere.</todo>
    public class OneDriveReader : ConsumerBase
    {
        /// <summary>
        /// Retrieves the names of the user's devices.
        /// </summary>
        /// <returns>Names of the user's devices.</returns>
        public async Task<string[]> GetDeviceNames()
            => (await this.Client.GetItemChildren("PassiveEyes"))
                .Where(item => item.Folder != null)
                .Select(item => item.Name)
                .ToArray();

        /// <summary>
        /// Retrieves the names of the camera streams under a device.
        /// </summary>
        /// <param name="deviceName">A name of a device.</param>
        /// <returns>Names of the device's camera streams.</returns>
        public async Task<string[]> GetCameraStreamNames(string deviceName)
            => (await this.Client.GetItemChildren($"PassiveEyes/{deviceName}"))
                .Where(item => item.Folder != null)
                .Select(item => item.Name)
                .ToArray();

        /// <summary>
        /// Retrieves <see cref="Item"/>s for recent photos from a camera stream.
        /// </summary>
        /// <param name="deviceName">The name of the camera stream's device.</param>
        /// <param name="cameraStreamName">The name of the camera stream.</param>
        /// <param name="cutoff">The earliest time to filter photos on.</param>
        /// <returns>Photos uploaded to the camera stream since the cutoff time.</returns>
        public async Task<Item[]> GetStreamPhotos(string deviceName, string cameraStreamName, DateTime cutoff)
            => (await this.Client.GetItemChildren(
                    $"PassiveEyes/{deviceName}/{cameraStreamName}",
                    this.FilterGenerator.GenerateFilters(
                        new[] { "createdDateTime", "gt", cutoff.Ticks.ToString() })));
    }
}
