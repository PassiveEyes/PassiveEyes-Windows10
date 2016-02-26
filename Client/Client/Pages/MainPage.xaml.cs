namespace Client.Pages
{
    using Models;
    using OneDrive;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using Windows.Foundation;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The redirect URI for OneDrive SDK authentication.
        /// </summary>
        private static readonly string RedirectUri = "https://login.live.com/oauth20_desktop.srf";

        /// <summary>
        /// Scopes allowed for OneDrive SDK authentication.
        /// </summary>
        private static readonly string[] Scopes = { "onedrive.readwrite", "wl.offline_access" };

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handler for the login/click button.
        /// </summary>
        /// <param name="sender">The triggered element.</param>
        /// <param name="e">The triggered event.</param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AuthWebView.Visibility = Visibility.Visible;
            string uri = "https://login.live.com/oauth20_authorize.srf?client_id="
                + Uri.EscapeDataString((App.Current as App).AppConfig.ClientId)
                + "&scope="
                + Uri.EscapeDataString(String.Join(" ", Scopes))
                + "&response_type=code&redirect_uri="
                + Uri.EscapeDataString(RedirectUri);
            System.Diagnostics.Debug.WriteLine(uri);
            AuthWebView.Navigate(new Uri(uri));
        }

        private async void AuthWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!args.Uri.ToString().StartsWith(RedirectUri))
            {
                return;
            }
            var queryDictionary = new WwwFormUrlDecoder(args.Uri.Query);
            if (args.Uri.Query.Substring(1).StartsWith("error"))
            {
                await new MessageDialog("Unable to authenticate with OneDrive.", "Whoops.").ShowAsync();
                AuthWebView.Visibility = Visibility.Collapsed;
                return;
            }
            var code = queryDictionary.GetFirstValueByName("code");
            var httpContent = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("client_id", (App.Current as App).AppConfig.ClientId),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                new KeyValuePair<string, string>("client_secret", (App.Current as App).AppConfig.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code)
            });
            var httpClient = new HttpClient();
            var result = await httpClient.PostAsync("https://login.live.com/oauth20_token.srf", httpContent);
            var contentStr = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
            {
                await new MessageDialog("Unable to authenticate with OneDrive.", "Whoops.").ShowAsync();
                AuthWebView.Visibility = Visibility.Collapsed;
                return;
            }
            var response = await result.Content.ReadAsAsync<AuthResponseModel>();
            ((App)Application.Current).Crap = new PieceOfCrap(response.AccessToken);
            await PieceOfCrap.RunAction(async (PieceOfCrap crap) => await crap.GetDrive());
            Frame.Navigate(typeof(UsageChoicePage));
        }
    }
}
