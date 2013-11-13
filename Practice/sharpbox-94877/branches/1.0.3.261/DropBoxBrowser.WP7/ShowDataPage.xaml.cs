using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox;
using Microsoft.Phone.Controls;

namespace DropBoxBrowser.WP7
{
    public partial class ShowDataPage : PhoneApplicationPage
    {
        private CloudStorage _storage;

        public ShowDataPage()
        {
            InitializeComponent();

            _storage = new CloudStorage();
        }

        private void listBox1_Loaded(object sender, RoutedEventArgs e)
        {           
            // set the title
            PageTitle.Text = "Loading...";

            const string consumerKey = "1kqqci5fkiw5yd9";
            const string consumerSecret = "qloes0saw19kpq6";

            ICloudStorageCredentials creds = null;
            string useToken = null;
            if (NavigationContext.QueryString.TryGetValue("UseToken", out useToken) && useToken.Equals("true"))
            {
                var accessToken = (App.Current as App).AccessToken;
                var dropboxCredsToken = new DropBoxCredentialsToken(consumerKey, consumerSecret, accessToken);

                creds = dropboxCredsToken;
            }
            else
            {
                // login            
                var dropboxCreds = new DropBoxCredentials();
                dropboxCreds.ConsumerKey = consumerKey;
                dropboxCreds.ConsumerSecret = consumerSecret;

                // get creds
                dropboxCreds.UserName = NavigationContext.QueryString["User"];
                dropboxCreds.Password = NavigationContext.QueryString["Password"];

                creds = dropboxCreds;
            }

            _storage.BeginOpenRequest(OpenAsyncCallback, DropBoxConfiguration.GetStandardConfiguration(), creds);
        }

        private void OpenAsyncCallback(IAsyncResult ar)
        {
            // this callback is part of the work thread form the pool
            // we have to invoke the ui calls in the ui thread

            // end the assync call and take the token
            ICloudStorageAccessToken token = _storage.EndOpenRequest(ar);

            // get the root, that works why we are in the worker thread
            ICloudDirectoryEntry root = _storage.GetRoot();

            // copy the result into result list
            // it's neceassary in case of the lacy loading feature
            List<ICloudFileSystemEntry> childs = new List<ICloudFileSystemEntry>();
            foreach (ICloudFileSystemEntry c in root)
                childs.Add(c);

            // transfer into ui thread and add the items
            Deployment.Current.Dispatcher.BeginInvoke( () =>
            {
                PageTitle.Text = "Root";

                foreach (ICloudFileSystemEntry entry in childs)
                {                    
                    lstItems.Items.Add(entry.Name);
                }

                // save the access token
                (App.Current as App).AccessToken = token;

                // save the stream out
                using (var stream = _storage.SerializeSecurityToken(token))
                {
                    // save as buffer
                    var tokenBuffer = new byte[stream.Length];
                    stream.Read(tokenBuffer, 0, tokenBuffer.Length);
                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    settings["dropboxTokenStream"] = tokenBuffer;
                    settings.Save();
                }
            });
        }

    }
}