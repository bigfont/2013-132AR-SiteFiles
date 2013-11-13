using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BoxNet;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class WebDavReferenceTestsSmartDrive : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDWebDavExternal);
            
            // build the credentials for the webdav server
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();            
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;
            
            Configuration = WebDavConfiguration.Get1and1Configuration();
            ConfigurationNonExistingServiceRoot = new WebDavConfiguration(new Uri(Configuration.ServiceLocator, Guid.NewGuid().ToString()));
        }
    }

    [TestFixture]
    public class WebDavReferenceTestsBoxNet : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDWebDavBoxNet);

            // build the credentials for the webdav server
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;

            Configuration = BoxNetConfiguration.GetBoxNetConfiguration();
        }
    }

    [TestFixture]
    public class WebDavReferenceTestsIIS7 : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDWebDavInternal);

            // build the credentials for the webdav server
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.WebDav, new Uri("http://127.0.0.1"));
            ((WebDavConfiguration)Configuration).UploadDataStreambuffered = true;
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class WebDavReferenceTestsIIS7PassThroughNTLM : GenericReferenceTests
    {
        public override void InitializeProvider()
        {            
            // build the credentials for the webdav server
            StorageProvider.GenericCurrentCredentials cred = new StorageProvider.GenericCurrentCredentials();
            
            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = "shs";
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.WebDav, new Uri("http://10.2.7.10:8080"));
            ((WebDavConfiguration)Configuration).UploadDataStreambuffered = true;
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class WebDavReferenceTestsStoreGate : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDWebDavStoreGate);

            // build the credentials for the webdav server
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.StoreGate, cred);            
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class WebDavReferenceTestsCloudMe : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDWebDavCloudMe);

            // build the credentials for the webdav server
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.CloudMe, cred);            
        }
    }

    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class WebDavReferenceTestsStratoHiDrive : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserHiDrive);

            // build the credentials for the webdav server
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.HiDrive);

            // special config for our UnitTests
            WebDavConfiguration cfg = (WebDavConfiguration)Configuration;
            cfg.ServiceLocator = new Uri(cfg.ServiceLocator, "users/" + cred.UserName);            
        }

        [Test()]
        [Category("ExcludedOnDeveloperMachine")]
        [Category("ExcludedOnAutoBuildMachine")]
        public override void FolderCreateAndReadEncodedFileNames()
        {
            throw new Exception("This feature is not support by HiDrive so this test should not be executed. Please use the exclude attributes");
        }
    }
}
