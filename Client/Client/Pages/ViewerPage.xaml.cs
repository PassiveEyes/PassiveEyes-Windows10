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

        public ViewerPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source
        /// of a parent Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

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
    }
}
