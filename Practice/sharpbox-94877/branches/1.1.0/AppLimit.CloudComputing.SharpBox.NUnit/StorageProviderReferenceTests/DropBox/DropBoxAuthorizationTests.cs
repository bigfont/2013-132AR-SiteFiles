using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using System.Net;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    [TestFixture]
    public class DropBoxReferenceTests : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account
            AccountDatabase DropBoxSecret = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appTagDropBox);
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDropBox);

            // build the credentials for dropbox
            StorageProvider.DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ConsumerSecret = DropBoxSecret.Password;

            StorageProvider.DropBox.DropBoxCredentials icred = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";
            icred.ConsumerKey = DropBoxSecret.User;
            icred.ConsumerSecret = DropBoxSecret.Password;

            Credentials = cred;
            InvalidCredentials = icred;
            Configuration = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();
        }

        [Test()]
        public void DropBoxGetAccountInfoTest()
        {
            DropBoxAccountInfo accInfo = DropBoxStorageProviderTools.GetAccountInformation(cloudStorage.CurrentAccessToken);

            Assert.IsNotEmpty(accInfo.Country);
            Assert.IsNotEmpty(accInfo.DisplayName);
            Assert.Greater(accInfo.UserId, 0);
            Assert.IsNotNull(accInfo.QuotaInfo);
            Assert.Greater(accInfo.QuotaInfo.NormalBytes, 0);
            Assert.Greater(accInfo.QuotaInfo.QuotaBytes, 0);
            Assert.Greater(accInfo.QuotaInfo.SharedBytes, 0);
        }

        [Test()]
        public void DropBoxFileUploadComfortRename2()
        {
            CloudStorage cs = new CloudStorage();
            cs.Open(Configuration, Credentials);

            ICloudFileSystemEntry fEntry = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                fEntry = cloudStorage.UploadFile("C:\\windows\\notepad.exe", "/Public", "newNamedFile");
            else
                fEntry = cloudStorage.UploadFile("/usr/bin/which", "/Public", "newNamedFile");

            // check
            Assert.AreEqual(fEntry.Name, "newNamedFile");

            // remove the file 
            cloudStorage.DeleteFileSystemEntry(fEntry);

            cs.Close();
        }

        [Test()]
        public void DropBoxGetPublicUri()
        {
            CloudStorage cs = new CloudStorage();
            ICloudStorageAccessToken tk = cs.Open(Configuration, Credentials);

            ICloudDirectoryEntry fEntry = cs.GetFolder("/Public");
            ICloudFileSystemEntry fs = cs.GetFileSystemObject("xxx.txt", fEntry);

            Uri uri = DropBoxStorageProviderTools.GetPublicObjectUrl(tk, fs);

            // download to temp
            WebClient cl = new WebClient();

            // target
            String temp = Environment.ExpandEnvironmentVariables("%temp%\\xxx.txt");


            // in the case of webdav we need to add the creds
            if (Credentials is GenericNetworkCredentials)
            {
                GenericNetworkCredentials creds = Credentials as GenericNetworkCredentials;
                cl.Credentials = creds.GetCredential(null, null);
            }

            cl.DownloadFile(uri, temp);

            // check if file exists
            Assert.That(File.Exists(temp));

            // remove the file
            File.Delete(temp);

            cs.Close();
        }
    }
}
