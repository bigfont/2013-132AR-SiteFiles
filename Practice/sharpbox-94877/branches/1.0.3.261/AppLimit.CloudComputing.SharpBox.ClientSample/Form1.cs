using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using AppLimit.CloudComputing.SharpBox.DropBox;
using AppLimit.Common.UI.Windows.Forms.Controller.TreeViewController;
using AppLimit.CloudComputing.SharpBox.ClientSample.Tasks;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.ClientSample
{
    public partial class Form1 : Form
    {
        private CommonUITreeViewController _treeViewCtrl;
        private AccountDatabase dropBoxSecrets;
        private AccountDatabase userAccount;

        private CloudStorage _cloudStorage;

        public Form1()
        {                        
            InitializeComponent();

            try
            {
                dropBoxSecrets = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp");
                userAccount = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropbox");

                edtUserName.Text = userAccount.User;
                edtPassword.Text = userAccount.Password;

                edtAppKey.Text = dropBoxSecrets.User;
                edtAppSecret.Text = dropBoxSecrets.Password;
            }
            catch (Exception)
            {                
            }

            
            _cloudStorage = new CloudStorage();

            _treeViewCtrl = new CommonUITreeViewController(treeView1, new CloudItemsLazyLoader(), CommonUITreeViewControllerBehaviour.Default);
            _treeViewCtrl.TreeViewData = _cloudStorage;
            _treeViewCtrl.TaskController.RegisterTask(typeof(CloudFolderShim), typeof(CreateDiretoryTask));
            _treeViewCtrl.TaskController.RegisterTask(typeof(CloudFolderShim), typeof(RemoveItemTask));
            _treeViewCtrl.TaskController.RegisterTask(typeof(CloudFolderShim), typeof(MoveToRootTask));
            
            lblStatus.Text = "";
        }

        private void btnLoadTree_Click(object sender, EventArgs e)
        {
            // init the background worker
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(LoadSandboxInTree);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(LoadSandBoxInTreeProgress);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadSandBoxInTreeFinished);

            // disable ui
            btnLoadTree.Enabled = false;
            treeView1.Enabled = false;
            edtUserName.Enabled = false;
            edtPassword.Enabled = false;

            // start the worker
            backgroundWorker1.RunWorkerAsync();
        }

        void LoadSandBoxInTreeFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            btnLoadTree.Enabled = true;
            treeView1.Enabled = true;
            edtUserName.Enabled = true;
            edtPassword.Enabled = true;

            lblStatus.Text = "";
            progressCurrent.Value = 0;
        }

        void LoadSandBoxInTreeProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                treeView1.Nodes.Clear();
                progressCurrent.Maximum = 3;
                progressCurrent.Minimum = 0;
                progressCurrent.Step = 1;
                progressCurrent.Value = 0;
            }
            else if (e.ProgressPercentage == 1)
            {
                lblStatus.Text = (String)e.UserState;
                progressCurrent.PerformStep();
            }
            else if (e.ProgressPercentage == 2)
            {
                var dir = e.UserState as ICloudDirectoryEntry;
                if (dir == null)
                    return;                
                
                _treeViewCtrl.PopulateObjectModel(new CloudFolderShim(dir, null));
            }                        
        }

        void LoadSandboxInTree(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0);
            backgroundWorker1.ReportProgress(1, "Configuring DropBox access");

            var configuration = DropBoxConfiguration.GetStandardConfiguration();
            var cred = new DropBoxCredentials();

            cred.ConsumerKey = edtAppKey.Text;
            cred.ConsumerSecret = edtAppSecret.Text;
            cred.UserName = edtUserName.Text;
            cred.Password = edtPassword.Text;
            
            backgroundWorker1.ReportProgress(1, "Opening DropBox (this takes a while)");

            _cloudStorage.Open(configuration, cred);            

            backgroundWorker1.ReportProgress(1, "Retrieving file information");
            var root = _cloudStorage.GetRoot();

            
                            
            backgroundWorker1.ReportProgress(2, root);                                        
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cloudStorage != null && _cloudStorage.IsOpened)
                _cloudStorage.Close();
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            // get the container shim
            //CloudFolderShim cfs = _treeViewCtrl.TreeViewSelectedObject as CloudFolderShim;

            // open the stream
            /*Stream fs = _cloudStorage.CreateFileStream(cfs as ICloudDirectoryEntry, edtFileName.Text, FileMode.Create, FileAccess.Write);

            // write the data
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine(edtFileContent.Text);
            writer.Close();

            // close the stream
            fs.Close();*/
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {            
            if (DialogResult == DialogResult.Cancel)
                Close();
        }          
    }
}
