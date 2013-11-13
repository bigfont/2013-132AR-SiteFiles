using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.MockProvider.Model;
using AppLimit.CloudComputing.SharpBox.StorageProvider.FTP;


namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    [TestFixture]
    public class FTPProviderTests : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // read the account            
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUserDFTP);
            
            // build the credentials for dropbox
            StorageProvider.GenericNetworkCredentials cred = new StorageProvider.GenericNetworkCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;

            StorageProvider.GenericNetworkCredentials icred = new StorageProvider.GenericNetworkCredentials();
            icred.UserName = UserAccount.User;
            icred.Password = "MyWrongPassword";
            
            Credentials = cred;
            InvalidCredentials = icred;

            // set the configuration
            Configuration = new FTPConfiguration("ftp://127.0.0.1");

            // register the exernal provider
            this.cloudStorage.RegisterStorageProvider(Configuration.GetType(), typeof(FTPStorageProvider));
        }       
    }
}
