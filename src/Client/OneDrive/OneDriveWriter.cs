using SDK.Media;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace PassiveEyes.SDK.OneDrive
{
    /// <summary>
    /// Writes folder metadata and newly captured photos to the user's OneDrive.
    /// </summary>
    public class OneDriveWriter : ConsumerBase
    {
        /// <summary>
        /// Asynchronously uploads a media capture to OneDrive as a new .jpg file.
        /// </summary>
        /// <param name="mediaCapture">An captured image from a camera stream.</param>
        /// <param name="state">The camera stream's activity state.</param>
        /// <param name="folderPath">Where to store the item in OneDrive.</param>
        /// <returns></returns>
        public async Task<string> UploadMediaCapture(MediaCapture mediaCapture, ActivityState state, string folderPath)
        {
            var fileName = this.GenerateFileName(state, DateTimeOffset.Now.Ticks);

            using (var fileStore = await TemporaryCaptureFileStore.Create(mediaCapture, fileName))
            {
                await this.Client.PutItem(folderPath, fileName, new StreamContent(fileStore.OutputStream));
                await fileStore.DisposeAsync();
            }

            return fileName;
        }

        /// <summary>
        /// Generates a file name for a new file based on the current time and name.
        /// </summary>
        /// <returns>A file name for a new file.</returns>
        private string GenerateFileName(ActivityState state, long timestamp)
            => $"PassiveEyes-{timestamp}-{state.ToString("G")}.jpg";
    }
}
