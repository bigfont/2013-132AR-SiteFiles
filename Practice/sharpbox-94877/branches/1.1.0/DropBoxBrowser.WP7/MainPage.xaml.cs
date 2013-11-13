using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using Microsoft.Phone.Controls;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace DropBoxBrowser.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {                
        const string consumerKey = "1kqqci5fkiw5yd9";
        const string consumerSecret = "qloes0saw19kpq6";

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
                        (App.Current as App).AccessToken = cl.DeserializeSecurityToken(stream, AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration());
                        Login((App.Current as App).AccessToken);
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
            //NavigationService.Navigate(new Uri("/ShowDataPage.xaml?Action=Login&User=" + edtAccount.Text + "&Password=" + edtPassword.Password, UriKind.Relative));            
            Login(edtAccount.Text, edtPassword.Password);
        }

        private void Login(String UserName, String Password)
        {
            // login            
            var dropboxCreds = new DropBoxCredentials();
            dropboxCreds.ConsumerKey = consumerKey;
            dropboxCreds.ConsumerSecret = consumerSecret;

            // get creds
            dropboxCreds.UserName = UserName;
            dropboxCreds.Password = Password;

            // get the standard config
            ICloudStorageConfiguration cfg = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // do it
            (App.Current as App).CloudAccess.BeginOpenRequest(LoginAsyncCallback, cfg, dropboxCreds);                        
        }

        private void Login(ICloudStorageAccessToken token)
        {
            // get the standard config
            ICloudStorageConfiguration cfg = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // open the storage
            (App.Current as App).CloudAccess.BeginOpenRequest(LoginAsyncCallback, cfg, token);            
        }

        void LoginAsyncCallback(IAsyncResult ar)
        {
            // end the assync call and take the token
            (App.Current as App).AccessToken = (App.Current as App).CloudAccess.EndOpenRequest(ar);

            if ((App.Current as App).AccessToken != null)
            {
                // set the root
                (App.Current as App).Root = (App.Current as App).CloudAccess.GetRoot();
                
                // save the stream out
                using (var stream = ((App.Current as App).CloudAccess.SerializeSecurityToken(((App.Current as App).AccessToken))))
                {
                    // save as buffer
                    var tokenBuffer = new byte[stream.Length];
                    stream.Read(tokenBuffer, 0, tokenBuffer.Length);
                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    settings["dropboxTokenStream"] = tokenBuffer;
                    settings.Save();
                }

                // navigate
                NavigateToDetails();
            }
        }

        void NavigateToDetails()
        {            
            if (Dispatcher.CheckAccess())
            {
                NavigationService.Navigate(new Uri("/ShowDataPage.xaml", UriKind.Relative));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(NavigateToDetails));
            }
        }
    }
}