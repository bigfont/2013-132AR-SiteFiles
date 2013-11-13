using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav;

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
            
            Credentials = cred;
            InvalidCredentials = icred;
            
            Configuration = WebDavConfiguration.Get1and1Configuration();            
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

            Credentials = cred;
            InvalidCredentials = icred;

            Configuration = WebDavConfiguration.GetBoxNetConfiguration();
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

            Credentials = cred;
            InvalidCredentials = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.WebDav, new Uri("http://127.0.0.1"));
            ((WebDavConfiguration)Configuration).UploadDataStreambuffered = true;
        }
    }

    [TestFixture]
    public class WebDavReferenceTestsIIS7PassThroughNTLM : GenericReferenceTests
    {
        public override void InitializeProvider()
        {            
            // build the credentials for the webdav server
            StorageProvider.GenericCurrentCredentials cred = new StorageProvider.GenericCurrentCredentials();
            
            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = "shs";
            icred.Password = "MyWrongPassword";

            Credentials = cred;
            InvalidCredentials = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.WebDav, new Uri("http://127.0.0.1"));
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

            Credentials = cred;
            InvalidCredentials = icred;

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

            Credentials = cred;
            InvalidCredentials = icred;

            Configuration = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.CloudMe, cred);
        }
    }
}
