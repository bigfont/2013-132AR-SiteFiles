﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using System.Net;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
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
            ConfigurationNonExistingServiceRoot = new DropBoxConfiguration(new Uri(Configuration.ServiceLocator, Guid.NewGuid().ToString()));
        }

        [Test()]
        public void DropBoxInvalidApplicationSecret()
        {
            // copy the credentails 
            DropBoxCredentials creds = new DropBoxCredentials();
            creds.UserName          = ((DropBoxCredentials)Credentials).UserName;
            creds.Password          = ((DropBoxCredentials)Credentials).Password;
            creds.ConsumerKey       = ((DropBoxCredentials)Credentials).ConsumerKey;
            creds.ConsumerSecret    = "MyWrongSecret";

            UnauthorizedAccessException ex = null;

            try
            {
                CloudStorage cs = new CloudStorage();
                cs.Open(Configuration, creds);
            }
            catch (UnauthorizedAccessException e)
            {
                ex = e;
            }

            Assert.IsNotNull(ex, "Expected unautheorized access exception");
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
            ICloudFileSystemEntry fs = cs.GetFileSystemObject("SharpBoxUnitTestPublicFile.txt", fEntry);

            Uri uri = DropBoxStorageProviderTools.GetPublicObjectUrl(tk, fs);

            // download to temp
            WebClient cl = new WebClient();

            // target
            String temp = Environment.ExpandEnvironmentVariables("%temp%\\SharpBoxUnitTestPublicFile.txt");


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

        [Test()]
        public void DropBoxVerifyFileAttributes()
        {         
            // get root
            ICloudDirectoryEntry root = cloudStorage.GetRoot();
            String hash = root.GetPropertyValue("hash");
            Assert.AreNotEqual(hash, "");

            String path = root.GetPropertyValue("path");
            Assert.AreEqual(path, "");

            // get Public folder            
            path = testRoot.GetPropertyValue("path");
            Assert.AreNotEqual(path, "");
        }

        [Test()]
        public void DropBoxSerializeRequestToken()
        {
            // create a cloudstorage for serialize
            CloudStorage cls = new CloudStorage();

            // load the dropbox secret
            AccountDatabase DropBoxSecret = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appTagDropBox);

            // build a request token
            DropBoxRequestToken requestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken((DropBoxConfiguration)cloudStorage.CurrentConfiguration, DropBoxSecret.User, DropBoxSecret.Password);
            
            // serialize the requestToken
            Stream tokenStream = cls.SerializeSecurityTokenEx(requestToken, typeof(DropBoxConfiguration), null);

            // deseri
            DropBoxRequestToken requestToken2 = cls.DeserializeSecurityToken(tokenStream) as DropBoxRequestToken;

            // check the test
            Assert.IsNotNull(requestToken2);            

            // create a authorzation url 
            String url = DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl((DropBoxConfiguration)cloudStorage.CurrentConfiguration, requestToken2);

            // check 
            Assert.IsNotEmpty(url);
        }
    }
}
