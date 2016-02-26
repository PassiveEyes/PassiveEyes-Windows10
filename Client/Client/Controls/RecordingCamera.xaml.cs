namespace Client.Controls
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Code-behind 
    /// </summary>
    public sealed partial class RecordingCamera : UserControl
    {
        public Camera Camera { get; set; }

        public RecordingCamera()
        {
            this.InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await this.Camera.Receiver.Initialize();
            this.PreviewControl.Source = this.Camera.Receiver.MediaCapture;
            await this.Camera.Receiver.MediaCapture.StartPreviewAsync();
        }
    }
}
