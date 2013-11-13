using System;
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
    public class DropBoxReferenceTestsFullBoxVersion0 : DropBoxReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the stored tokens
            ValidAccessToken = AccountDatabaseEx.GetTokenFile("validDropBoxToken.token");
            InvalidAccessToken = AccountDatabaseEx.GetTokenFile("invalidDropBoxToken.token");

            // get the config
            Configuration = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();
            
            // set the api version to 0
            ((DropBoxConfiguration)Configuration).APIVersion = DropBoxAPIVersion.V0;

            // configure our helpers
            ConfigurationNonExistingServiceRoot = new DropBoxConfiguration(new Uri(Configuration.ServiceLocator, Guid.NewGuid().ToString()));
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class DropBoxReferenceTestsSandBoxVersion0 : DropBoxReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the stored tokens
            ValidAccessToken = AccountDatabaseEx.GetTokenFile("validDropBoxSandBox.token");
            InvalidAccessToken = AccountDatabaseEx.GetTokenFile("invalidDropBoxToken.token");

            // get the config
            Configuration = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // set the api version to 0
            ((DropBoxConfiguration)Configuration).APIVersion = DropBoxAPIVersion.V0;

            // configure our helpers
            ConfigurationNonExistingServiceRoot = new DropBoxConfiguration(new Uri(Configuration.ServiceLocator, Guid.NewGuid().ToString()));
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class DropBoxReferenceTestsVersion1 : DropBoxReferenceTests
    {
        public override void InitializeProvider()
        {            
            // read the stored tokens
            ValidAccessToken = AccountDatabaseEx.GetTokenFile("validDropBoxToken.token");
            InvalidAccessToken = AccountDatabaseEx.GetTokenFile("invalidDropBoxToken.token");

            // get the config
            Configuration = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // set the api version to 1
            ((DropBoxConfiguration)Configuration).APIVersion = DropBoxAPIVersion.V1;

            // configure our helpers
            ConfigurationNonExistingServiceRoot = new DropBoxConfiguration(new Uri(Configuration.ServiceLocator, Guid.NewGuid().ToString()));
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class DropBoxReferenceTestsVersion1Sandbox : DropBoxReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the stored tokens
            ValidAccessToken = AccountDatabaseEx.GetTokenFile("validDropBoxSandBox.token");
            InvalidAccessToken = AccountDatabaseEx.GetTokenFile("invalidDropBoxToken.token");

            // get the config
            Configuration = AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // set the api version to 0
            ((DropBoxConfiguration)Configuration).APIVersion = DropBoxAPIVersion.V1;

            // configure our helpers
            ConfigurationNonExistingServiceRoot = new DropBoxConfiguration(new Uri(Configuration.ServiceLocator, Guid.NewGuid().ToString()));
        }

        [Test()]
        public override void DropBoxGetPublicUri()
        {
            // nothing to do because in sandbox mode it's not possibel to get a public uri!
        }
        
    }

    /// <summary>
    /// Our real test class
    /// </summary>
    public abstract class DropBoxReferenceTests : GenericReferenceTests
    {       
        [Test()]
        public void DropBoxInvalidApplicationSecret()
        {
            // load a special prepared token
            ICloudStorageAccessToken specialToken = AccountDatabaseEx.GetTokenFile("invalidConsumerSecretDropBoxToken.token");
            
            UnauthorizedAccessException ex = null;

            try
            {
                CloudStorage cs = new CloudStorage();
                cs.Open(Configuration, specialToken);
            }
            catch (UnauthorizedAccessException e)
            {
                ex = e;
            }

            Assert.IsNotNull(ex, "Expected unauthorized access exception");
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
            cs.Open(Configuration, ValidAccessToken);
            
            // check public folder
            Boolean bPublicCreated = false;
            ICloudDirectoryEntry fPublic = cloudStorage.GetFolder("/Public", false);
            if (fPublic == null)
            {
                fPublic = cloudStorage.CreateFolder("/Public");
                bPublicCreated = true;
            }

            // lets do it
            ICloudFileSystemEntry fEntry = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                fEntry = cloudStorage.UploadFile("C:\\windows\\notepad.exe", "/Public", "newNamedFile");
            else
                fEntry = cloudStorage.UploadFile("/usr/bin/which", "/Public", "newNamedFile");

            // check
            Assert.AreEqual(fEntry.Name, "newNamedFile");

            // remove the file 
            cloudStorage.DeleteFileSystemEntry(fEntry);

            if (bPublicCreated)
                cloudStorage.DeleteFileSystemEntry(fPublic);

            cs.Close();
        }

        [Test()]
        public virtual void DropBoxGetPublicUri()
        {
            CloudStorage cs = new CloudStorage();
            ICloudStorageAccessToken tk = cs.Open(Configuration, ValidAccessToken);

            // no public we are in sandbox mode
            ICloudDirectoryEntry fEntry = cs.GetFolder("/Public");            
            ICloudFileSystemEntry fs = cs.GetFileSystemObject("SharpBoxUnitTestPublicFile.txt", fEntry);

            Uri uri = DropBoxStorageProviderTools.GetPublicObjectUrl(tk, fs);

            // download to temp
            WebClient cl = new WebClient();

            // target
            String temp = Environment.ExpandEnvironmentVariables("%temp%\\SharpBoxUnitTestPublicFile.txt");


            // in the case of webdav we need to add the creds
            if (ValidAccessToken is GenericNetworkCredentials)
            {
                GenericNetworkCredentials creds = ValidAccessToken as GenericNetworkCredentials;
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
