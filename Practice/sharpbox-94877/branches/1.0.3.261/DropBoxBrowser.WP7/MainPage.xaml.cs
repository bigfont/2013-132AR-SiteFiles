using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using Microsoft.Phone.Controls;

namespace DropBoxBrowser.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {        
        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }        

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // check to see if there is a token
            byte[] tokenBuffer;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.TryGetValue("dropboxTokenStream", out tokenBuffer))
            {
                try
                {
                    // login with the token information
                    using (var stream = new MemoryStream(tokenBuffer))
                    {
                        CloudStorage cl = new CloudStorage();
                        (App.Current as App).AccessToken = cl.DeserializeSecurityToken(stream, AppLimit.CloudComputing.SharpBox.DropBox.DropBoxConfiguration.GetStandardConfiguration());
                        NavigationService.Navigate(new Uri("/ShowDataPage.xaml?Action=Login&UseToken=true", UriKind.Relative));
                    }
                }
                catch (Exception ex)
                {
                    // ignore - best effort
                    Debug.WriteLine("Exception reading dropboxTokenStream: " + ex);
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {                        
            NavigationService.Navigate(new Uri("/ShowDataPage.xaml?Action=Login&User=" + edtAccount.Text + "&Password=" + edtPassword.Password, UriKind.Relative));            
        }
    }
}