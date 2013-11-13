using System;
using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
	[TestFixture()]
	public class DropBoxAuthorizationTests
	{
		[Test()]
		public void LoginLogOffTestDropBox()
        {
            // read the account
            AccountDatabase DropBoxSecret 	= AccountDatabaseEx.GetAccount(AccountDatabaseEx.appTag);
			AccountDatabase UserAccount 	= AccountDatabaseEx.GetAccount(AccountDatabaseEx.appUser);
			
            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ConsumerSecret = DropBoxSecret.Password;

            // open the dropbox connection
			CloudStorage cloudStorage = new CloudStorage();
			
			DropBox.DropBoxConfiguration dpConfig = DropBox.DropBoxConfiguration.GetStandardConfiguration();
			
            Boolean bret = cloudStorage.Open(dpConfig, cred);

            // check the result
            Assert.IsTrue(bret, "Dropbox login failed");
						
            // close the connection
            cloudStorage.Close();

            // check the result
            Assert.IsFalse(cloudStorage.IsOpened, "Dropbox logoff failed");
        }

		[Test()]
		public void MultiLoginTestDropBox()
        {
  			// read the account
            AccountDatabase DropBoxSecret 	= AccountDatabaseEx.GetAccount("dropboxapp");
			AccountDatabase UserAccount 	= AccountDatabaseEx.GetAccount("dropbox");
			
            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ConsumerSecret = DropBoxSecret.Password;

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

