using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace TutApp01
{
    class Program
    {
        static void Main(string[] args)
        {
            // Creating the cloudstorage object
            CloudStorage dropBoxStorage = new CloudStorage();

            // get the configuration for dropbox 
            var dropBoxConfig = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);

            // building credentials for the service
            var dropBoxCredentials = new DropBoxCredentials();
            
            // set the information you got from the dropbox team 
            dropBoxCredentials.ConsumerKey = "<<YOURAPIKEY>>";
            dropBoxCredentials.ConsumerSecret = "<<YOURAPISECRET";

            // set the information you got from the enduser
            dropBoxCredentials.UserName = "<<ENDUSER DROPBOX ACCOUNT>>";
            dropBoxCredentials.Password = "<<ENDUSER DROPBOX PASSWORD>>";

            // open the connection 
            var storageToken = dropBoxStorage.Open(dropBoxConfig, dropBoxCredentials);

            //
            // do what ever you want to 
            //

            // get a specific directory in the cloud storage, e.g. /Public
            var publicFolder = dropBoxStorage.GetFolder("/Public");

            // enumerate all child (folder and files)
            foreach (var fof in publicFolder)
            {
                // check if we have a directory
                Boolean bIsDirectory = fof is ICloudDirectoryEntry;

                // output the info
                Console.WriteLine("{0}: {1}", bIsDirectory ? "DIR" : "FIL", fof.Name );
            }

            // upload a testfile from temp directory into public folder of DropBox
            String srcFile = Environment.ExpandEnvironmentVariables("%temp%\\test.dat");
            dropBoxStorage.UploadFile(srcFile, publicFolder);

            // download the testfile from DropBox
            dropBoxStorage.DownloadFile(publicFolder, "test.dat", Environment.ExpandEnvironmentVariables("%temp%"));

            // upload a testfile from temp directory into public folder of DropBox 
            // with progress information
            dropBoxStorage.UploadFile(srcFile, publicFolder, UploadDownloadProgress);

            // download the testfile from DropBox with progress information
            dropBoxStorage.DownloadFile(publicFolder, "test.dat", Environment.ExpandEnvironmentVariables("%temp%"), UploadDownloadProgress);

            // close the connection 
            dropBoxStorage.Close();
        }

        static void UploadDownloadProgress(Object sender, FileDataTransferEventArgs e)
        {
            // print a dot           
            Console.Write(".");

            // it's ok to go forward
            e.Cancel = false;
        }
    }
}
