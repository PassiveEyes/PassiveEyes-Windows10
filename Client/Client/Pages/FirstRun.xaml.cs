namespace Client.Pages
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRun : Page
    {
        /// <summary>
        /// Initializes a new instance of the FirstRun class.
        /// </summary>
        public FirstRun()
        {
            this.InitializeComponent();

            this.RefreshMessage();
        }

        private void RefreshMessage()
        {
            if (((App)Application.Current).StorageFolderPath == null)
            {
                this.Message.Text = "You haven't used PassiveEyes before. Select where to store photos.";
            }
            else
            {
                this.Message.Text = "You've used PassiveEyes before. Confirm this is where you want to store photos.";
                this.StorageFolderPathInput.Text = ((App)Application.Current).StorageFolderPath;
            }
        }

        private void StorageFolderPathInputButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).StorageFolderPath = this.StorageFolderPathInput.Text;

            Frame.Navigate(typeof(RecordingPage));
        }
    }
}
