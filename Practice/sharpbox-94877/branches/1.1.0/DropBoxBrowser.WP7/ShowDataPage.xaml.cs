using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using Microsoft.Phone.Controls;

namespace DropBoxBrowser.WP7
{
    public partial class ShowDataPage : PhoneApplicationPage
    {       
        public ShowDataPage()
        {
            InitializeComponent();            
        }

        private void listBox1_Loaded(object sender, RoutedEventArgs e)
        {           
            // set the title
            PageTitle.Text = "Loading...";

            // perform the load child bckground operation
            (App.Current as App).CloudAccess.BeginGetChildsRequest(ChildsAsyncCallback, (App.Current as App).Root);                        
        }

        private void ChildsAsyncCallback(IAsyncResult ar)
        {
            // this callback is part of the work thread form the pool
            // we have to invoke the ui calls in the ui thread

            // end the assync call and take the token
            List<ICloudFileSystemEntry> l = (App.Current as App).CloudAccess.EndGetChildsRequest(ar);
            
            // transfer into ui thread and add the items
            Deployment.Current.Dispatcher.BeginInvoke( () =>
            {
                PageTitle.Text = "Root";

                foreach (ICloudFileSystemEntry entry in l)
                {                    
                    lstItems.Items.Add(entry.Name);
                }                
            });
        }

    }
}