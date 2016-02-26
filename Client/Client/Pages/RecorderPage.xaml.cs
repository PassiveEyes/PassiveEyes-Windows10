namespace Client.Pages
{
    using Models;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A dashboard for all webcams, with video previews and recording togglers.
    /// </summary>
    public sealed partial class RecorderPage : Page
    {
        /// <summary>
        /// The code-behind view model.
        /// </summary>
        public ReorderPageViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderPage"/> class.
        /// </summary>
        public RecorderPage()
        {
            this.ViewModel = new ReorderPageViewModel();
            this.InitializeComponent();
        }

        /// <summary>
        /// Reacts to the page being navigated to.
        /// </summary>
        /// <param name="e">The triggering event.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.Initialize();
        }

        /// <summary>
        /// Navigates to the <see cref="UsageChoicePage"/>.
        /// </summary>
        private void AppBarButton_Click(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(UsageChoicePage));
    }
}
