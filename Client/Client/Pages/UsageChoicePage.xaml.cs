namespace Client.Pages
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A simple page to choose between recording or controlling.
    /// </summary>
    public sealed partial class UsageChoicePage : Page
    {
        public UsageChoicePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Launches the <see cref="RecorderPage"/> page.
        /// </summary>
        private void RecorderChooser_Click(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(RecorderPage));

        /// <summary>
        /// Launches the <see cref="Controller"/> page.
        /// </summary>
        private void ViewerChooser_Click(object sender, RoutedEventArgs e) => Frame.Navigate(typeof(ViewerPage));
    }
}
