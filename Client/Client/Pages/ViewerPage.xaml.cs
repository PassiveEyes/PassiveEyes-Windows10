namespace Client.Pages
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading;
    using System.Threading.Tasks;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewerPage : Page
    {
        public ViewerViewModel ViewModel { get; private set; } = new ViewerViewModel();

        public ViewerPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Reacts to the page being navigated to.
        /// </summary>
        /// <param name="e">The triggering event.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.Initialize();

            await Task.Delay(1000);

            this.OnNavigatedTo(e);
        }

        private void Reload(object param)
        {
            var type = this.Frame.CurrentSourcePageType;

            if (this.Frame.BackStack.Any())
            {
                type = this.Frame.BackStack.Last().SourcePageType;
                param = this.Frame.BackStack.Last().Parameter;
            }

            try
            {
                this.Frame.Navigate(type, param);
                return;
            }
            finally
            {
                this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
            }
        }
    }
}
