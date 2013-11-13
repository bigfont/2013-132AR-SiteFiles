using System;
using System.Windows.Forms;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.ClientSample
{
    public partial class SharpBoxUI_Sample : Form
    {
        private CloudStorage _storage;
        private ICloudStorageAccessToken _storageToken;
        private ICloudStorageConfiguration _storageConfiguration;
        private DropBoxCredentials _storageCredentials;

        public SharpBoxUI_Sample()
        {
            InitializeComponent();
        }

        private void SharpBoxUI_Sample_Load(object sender, EventArgs e)
        {
            if (_storageConfiguration == null)
            {
                _storageConfiguration = DropBoxConfiguration.GetStandardConfiguration();
            }
        }

        private void loginControl1_LoginCompleted(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (_storageCredentials == null)
                {
                    _storageCredentials = new DropBoxCredentials();

                    // Configuration located in the same place of trunk
                    AccountDatabase dropBoxSecrets = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\Configuration\\accounts.xml", "dropboxapp");
                    _storageCredentials.ConsumerKey = dropBoxSecrets.User;
                    _storageCredentials.ConsumerSecret = dropBoxSecrets.Password;

                    try
                    {
                        if (string.IsNullOrWhiteSpace(loginControl1.User))
                        {
                            AccountDatabase dropBoxCredentials = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\Configuration\\accounts.xml", "dropboxcred");
                            _storageCredentials.UserName = dropBoxCredentials.User;
                            _storageCredentials.Password = dropBoxCredentials.Password;
                        }
                        else
                        {
                            _storageCredentials.UserName = loginControl1.User;
                            _storageCredentials.Password = loginControl1.Password;
                        }
                    }
                    catch
                    {
                        _storageCredentials = new DropBoxCredentials();
                        throw new Exception("Credentials have not been given!");
                    }
                }

                if (_storage == null)
                {
                    _storage = new CloudStorage();
                }
                if (_storageToken == null)
                {
                    _storageToken = _storage.Open(_storageConfiguration, _storageCredentials);
                }
                else
                {
                    _storage.Open(_storageConfiguration, _storageToken);
                }

                sandbox1.SetCloudStorage(_storage);
                sandbox1.LoadSandBox();

                groupBox2.Enabled = true;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _storage.Open(_storageConfiguration, _storageToken);
            sandbox1.LoadSandBox();
        }

        private void sandbox1_SandboxLoadCompleted(object sender, EventArgs e)
        {
            _storage.Close();
        }
    }
}
