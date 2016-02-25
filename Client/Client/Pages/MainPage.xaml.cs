﻿namespace Client.Pages
{
    using OneDrive;
    using System;
    using System.Diagnostics;
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
        private static readonly string RedirectUri = "";

        /// <summary>
        /// Scopes allowed for OneDrive SDK authentication.
        /// </summary>
        private static readonly string[] Scopes = { "onedrive.readwrite", "wl.signin" };

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
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var crap = ((App)Application.Current).Crap = new PieceOfCrap();

            await PieceOfCrap.RunAction(async () => await crap.GetDrive());

            Frame.Navigate(typeof(FirstRun));
        }
    }
}
