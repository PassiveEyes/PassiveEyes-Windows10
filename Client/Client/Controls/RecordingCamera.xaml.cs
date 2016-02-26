namespace Client.Controls
{
    using Models;
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Code-behind for the <see cref="RecordingCamera"/> class.
    /// </summary>
    public sealed partial class RecordingCamera : UserControl
    {
        /// <summary>
        /// A camera providing the media source.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingCamera"/> class.
        /// </summary>
        public RecordingCamera()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Responds to the control loading by initializing the media source preview.
        /// </summary>
        /// <param name="sender">The <see cref="RecordingCamera"/> control.</param>
        /// <param name="e">The triggering event arguments.</param>
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await this.Camera.Receiver.Initialize();
            this.PreviewControl.Source = this.Camera.Receiver.MediaCapture;
            await this.Camera.Receiver.MediaCapture.StartPreviewAsync();
        }
    }
}
