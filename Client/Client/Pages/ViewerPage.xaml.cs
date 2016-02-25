using Client.Models;
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

namespace Client.Pages
{
    public sealed partial class ViewerPage : Page
    {
        /// <summary>
        /// A reference to the app's view model for data binding.
        /// </summary>
        public ViewModel ViewModel
        {
            get
            {
                return (Application.Current as App).ViewModel;
            }
        }

        public ViewerPage()
        {
            this.InitializeComponent();
        }

        private void FeedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedFeed = FeedList.SelectedItem as FeedModel;
        }
    }
}
