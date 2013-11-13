using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.IntegrationTests.UnitTestClasses;

namespace AppLimit.CloudComputing.SharpBox.IntegrationTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AuthorizationTest : SharpBoxTestItem
    {        
        public AuthorizationTest()
        {                      
        }

        [Priority(1), TestMethod]
        public void LoginLogOffTestDropBox()
        {
            // read the account
            AccountDatabase DropBoxSecret = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp");
            AccountDatabase UserAccount = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropbox");

            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ComsumerSecret = DropBoxSecret.Password;

            // open the dropbox connection
            Boolean bret = this.CloudStorage.Open(DropBox.DropBoxConfiguration.GetStandardConfiguration(), cred);

            // check the result 
            Assert.IsTrue(bret, "Dropbox login failed");

            // close the connection
            CloudStorage.Close();

            // check the result 
            Assert.IsFalse(CloudStorage.IsOpened, "Dropbox logoff failed");
        }

        [Priority(2), TestMethod]
        public void MultiLoginTestDropBox()
        {
            // read the account
            AccountDatabase DropBoxSecret = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp");
            AccountDatabase UserAccount = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropbox");

            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ComsumerSecret = DropBoxSecret.Password;

            DropBox.DropBoxConfiguration conf = DropBox.DropBoxConfiguration.GetStandardConfiguration();
            conf.HasToShowAuthorizationProcess = true;

            // open the dropbox connection 1
            Boolean bret = false;
            CloudStorage storage1 = new CloudStorage();
            bret = storage1.Open(conf, cred);

            // check the result 
            Assert.IsTrue(bret, "Dropbox 1 login failed");

            // open the dropbox connection 2
            CloudStorage storage2 = new CloudStorage();
            bret = storage2.Open(conf, cred);

            // check the result 
            Assert.IsTrue(bret, "Dropbox 2 login failed");

            // close the conn
            if (storage1.IsOpened)
                storage1.Close();


            if (storage2.IsOpened)
                storage2.Close();

        }
        
    }
}
