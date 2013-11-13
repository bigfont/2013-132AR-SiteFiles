using System;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
	[TestFixture()]
    public class DropBoxAuthorizationTests : OAuthTraceableTest
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
			
            // will thrown an exception
            cloudStorage.Open(dpConfig, cred);
						
            // close the connection
            cloudStorage.Close();

            // check the result
            Assert.IsFalse(cloudStorage.IsOpened, "Dropbox logoff failed");
        }

        [Test()]
        public void LoginLogOffTestDropBox20()
        {
            for(int i = 0; i < 20; i++)
                LoginLogOffTestDropBox();
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
            CloudStorage storage1 = new CloudStorage();            
            storage1.Open(conf, cred);
            
            // open the dropbox connection 2
            CloudStorage storage2 = new CloudStorage();
            storage2.Open(conf, cred);
            
            // close the conn
            if (storage1.IsOpened)
                storage1.Close();

            if (storage2.IsOpened)
                storage2.Close();
        }

        [Test()]
        public void WrongPasswordTest()
        {
            // read the account
            AccountDatabase DropBoxSecret = AccountDatabaseEx.GetAccount("dropboxapp");
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount("dropbox");

            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = "MyWrongPassword";
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ConsumerSecret = DropBoxSecret.Password;

            DropBox.DropBoxConfiguration conf = DropBox.DropBoxConfiguration.GetStandardConfiguration();            

            // open the dropbox connection
            Boolean bGotException = false;
            CloudStorage storage1 = new CloudStorage();

            // check throw exceotion
            try
            {
                storage1.Open(conf, cred);
            }
            catch (System.UnauthorizedAccessException)
            {
                bGotException = true;
                Assert.IsFalse(storage1.IsOpened, "Wrong is opened status");
            }
            
            // check the result 
            Assert.IsTrue(bGotException, "No exception detected");            
        }


        [Test()]
        public void TokenAccessTest()
        {
            // read the account
            AccountDatabase DropBoxSecret = AccountDatabaseEx.GetAccount("dropboxapp");
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount("dropbox");

            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ConsumerSecret = DropBoxSecret.Password;

            DropBox.DropBoxConfiguration conf = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage();
            ICloudStorageAccessToken token = storage1.Open(conf, cred);
            storage1.Close();

            // build token based credentials
            DropBox.DropBoxCredentialsToken credToken = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentialsToken();
            credToken.ConsumerKey = DropBoxSecret.User;
            credToken.ConsumerSecret = DropBoxSecret.Password;
            credToken.AccessToken = token;

            // open with token
            CloudStorage storage2 = new CloudStorage();
            storage2.Open(conf, credToken);

            // read out the root
            ICloudDirectoryEntry root = storage2.GetRoot();
            Assert.IsNotNull(root);
            Assert.Greater(root.Count, 0);

            storage2.Close();
        }

        [Test()]
        public void TokenStoreTest()
        {
            // read the account
            AccountDatabase DropBoxSecret = AccountDatabaseEx.GetAccount("dropboxapp");
            AccountDatabase UserAccount = AccountDatabaseEx.GetAccount("dropbox");

            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ConsumerSecret = DropBoxSecret.Password;

            DropBox.DropBoxConfiguration conf = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage();
            ICloudStorageAccessToken token = storage1.Open(conf, cred);

            // store the token in a stream
            Stream tokenStream = storage1.SerializeSecurityToken(token);
            
            storage1.Close();

            // generate token from stream
            CloudStorage storage2 = new CloudStorage();

            ICloudStorageAccessToken token2 =  storage2.DeserializeSecurityToken(tokenStream, conf);

            // open the dropbox connection 2              
            DropBox.DropBoxCredentialsToken credToken = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentialsToken();
            credToken.ConsumerKey = DropBoxSecret.User;
            credToken.ConsumerSecret = DropBoxSecret.Password;
            credToken.AccessToken = token2;
            storage2.Open(conf, credToken);

            // read out the root
            ICloudDirectoryEntry root = storage2.GetRoot();
            Assert.IsNotNull(root);
            Assert.Greater(root.Count, 0);

            storage2.Close();


        }

	}
}

