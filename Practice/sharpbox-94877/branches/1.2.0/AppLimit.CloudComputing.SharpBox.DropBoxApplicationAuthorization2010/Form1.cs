using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.DropBoxApplicationAuthorization
{
    public partial class Form1 : Form
    {
        private DropBoxConfiguration _UsedConfig = null;
        private DropBoxRequestToken _CurrentRequestToken = null;
        private ICloudStorageAccessToken _GeneratedToken = null;

        public Form1()
        {
            InitializeComponent();

            // get a standard config
            _UsedConfig = DropBoxConfiguration.GetStandardConfiguration();


            webBrowser.DocumentTitleChanged += new EventHandler(webBrowser_DocumentTitleChanged);
        }        

        /// <summary>
        /// Starts the token exchange process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, EventArgs e)
        {
            // 0. reset token
            _GeneratedToken = null;

            // 1. modify dropbox configuration            
            _UsedConfig.APIVersion = DropBoxAPIVersion.V1;            

            // 2. get the request token
            _CurrentRequestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken(_UsedConfig, edtAppKey.Text, edtAppSecret.Text);

            // 3. get the authorization url 
            String AuthUrl = DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl(_UsedConfig, _CurrentRequestToken);

            // 4. naviagte to the AuthUrl 
            webBrowser.Navigate(AuthUrl);
        }

        /// <summary>
        /// finishes the token exchange process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void webBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            if (_GeneratedToken == null && webBrowser.Url.ToString().StartsWith(_UsedConfig.AuthorizationCallBack.ToString()))
            {
                // 5. try to get the real token
                _GeneratedToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(_UsedConfig, edtAppKey.Text, edtAppSecret.Text, _CurrentRequestToken);

                // 6. store the real token to file
                CloudStorage cs = new CloudStorage();
                cs.SerializeSecurityTokenEx(_GeneratedToken, _UsedConfig.GetType(), null, edtOutput.Text);
                
                // 7. show message box
                MessageBox.Show("Stored token into " + edtOutput.Text);                
            }        
        }

        private void btnTestToken_Click(object sender, EventArgs e)
        {
            // check
            if (edtOutput.Text.Length == 0)
            {
                MessageBox.Show("Sorry could not find a token path in the output box");
                return;
            }

            try
            {
                // try to load the token from file
                CloudStorage cs = new CloudStorage();
                ICloudStorageAccessToken accessToken = cs.DeserializeSecurityTokenEx(edtOutput.Text);

                // get the right config
                ICloudStorageConfiguration cfg = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);

                // try to open the box
                cs.Open(cfg, accessToken);

                // check if opened
                if (cs.IsOpened)
                {
                    ICloudDirectoryEntry entry = cs.GetRoot();
                    MessageBox.Show("Found your root with " + entry.Count + " childs");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry, something went wrong: " + ex.Message);
            }

        }

        private void btnSearchFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                edtOutput.Text = dlg.FileName;
            }
        }
    }
}
