using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Collections.Generic;
using AppLimit.CloudComputing.SharpBox;
using Microsoft.Phone.Controls;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav;
using AppLimit.CloudComputing.SharpBox.StorageProvider;

namespace SharpBoxBrowser.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {                
        const string consumerKey = "1kqqci5fkiw5yd9";
        const string consumerSecret = "qloes0saw19kpq6";
        const string configurltag = "ConfigUrl";
        const string isolatedStorageName = "tokenstore";

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }        

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // set the standard values            
            edtUrl.Text = "<<DropBox, no URL needed>>";
            rdDropBox.IsChecked = false;
            rdWebDav.IsChecked = true;

            ICloudStorageConfiguration cfgx = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.SmartDrive);
            edtUrl.Text = cfgx.ServiceLocator.ToString();
            
            // check to see if there is a token
            byte[] tokenBuffer;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.TryGetValue(isolatedStorageName, out tokenBuffer))
            {
                try
                {
                    // login with the token information
                    using (var stream = new MemoryStream(tokenBuffer))
                    {
                        // build a new cloud storage
                        CloudStorage cl = new CloudStorage();

                        // deserialize the token
                        Dictionary<String, String> meta = null;
                        (App.Current as App).AccessToken = cl.DeserializeSecurityToken(stream, out meta);

                        // get the url meta data
                        String url = meta[configurltag];

                        // create config by url
                        ICloudStorageConfiguration cfg = null;

                        // login with config 
                        Login(cfg, (App.Current as App).AccessToken);
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
            ICloudStorageConfiguration cfg = null;

            if (rdDropBox.IsChecked == true)
                cfg = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);
            else
                cfg = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.WebDav, new Uri(edtUrl.Text));

                        
            Login(cfg, edtAccount.Text, edtPassword.Password);
        }

        private void Login(ICloudStorageConfiguration cfg, String UserName, String Password)
        {
            // login            
            ICloudStorageCredentials creds = null;

            if (cfg is AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav.WebDavConfiguration)
            {
                GenericNetworkCredentials wcreds = new GenericNetworkCredentials();
                wcreds.UserName = UserName;
                wcreds.Password = Password;

                creds = wcreds;
            }
            else
            {
                DropBoxCredentials dcreds = new DropBoxCredentials();
                dcreds.ConsumerKey = consumerKey;
                dcreds.ConsumerSecret = consumerSecret;

                // get creds
                dcreds.UserName = UserName;
                dcreds.Password = Password;

                creds = dcreds;
            }
            
            // do it
            (App.Current as App).CloudAccess.BeginOpenRequest(LoginAsyncCallback, cfg, creds);                        
        }

        private void Login(ICloudStorageConfiguration Configuration, ICloudStorageAccessToken token)
        {            
            // open the storage
            (App.Current as App).CloudAccess.BeginOpenRequest(LoginAsyncCallback, Configuration, token);            
        }

        void LoginAsyncCallback(IAsyncResult ar)
        {
            // end the assync call and take the token
            (App.Current as App).AccessToken = (App.Current as App).CloudAccess.EndOpenRequest(ar);

            if ((App.Current as App).AccessToken != null)
            {               
                // set the metadata
                Dictionary<String, String> metadata = new Dictionary<string, string>();
                metadata.Add(configurltag, (App.Current as App).CloudAccess.CurrentConfiguration.ServiceLocator.ToString());

                // save the stream out
                using (var stream = ((App.Current as App).CloudAccess.SerializeSecurityToken(((App.Current as App).AccessToken))))
                {
                    // save as buffer
                    var tokenBuffer = new byte[stream.Length];
                    stream.Read(tokenBuffer, 0, tokenBuffer.Length);
                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    settings[isolatedStorageName] = tokenBuffer;
                    settings.Save();
                }

                // get root
                (App.Current as App).CloudAccess.BeginGetRootRequest(GetRootAsyncCallback);                        
            }
        }

        void GetRootAsyncCallback(IAsyncResult ar)
        {
            // set the root
            (App.Current as App).Root = (App.Current as App).CloudAccess.EndGetRootRequest(ar);

            if ((App.Current as App).Root != null)
            {
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

        private void rdWebDav_Checked(object sender, RoutedEventArgs e)
        {
            if (edtUrl != null)
                edtUrl.Text = "";
        }

        private void rdDropBox_Checked(object sender, RoutedEventArgs e)
        {
            if (edtUrl != null )
                edtUrl.Text = "<<DropBox, no URL needed>>";
        }
    }
}