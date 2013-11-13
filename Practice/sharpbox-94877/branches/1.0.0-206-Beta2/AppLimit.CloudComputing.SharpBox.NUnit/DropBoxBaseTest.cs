using System;
using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
	public class DropBoxBaseTest
	{
		protected CloudStorage CloudStorage;
		
		public DropBoxBaseTest ()
		{
		}
		
		[TestFixtureSetUpAttribute]
		public void SetupFixture()
		{
			 // read the account
            AccountDatabase DropBoxSecret 	= AccountDatabaseEx.GetAccount("dropboxapp");
			AccountDatabase UserAccount 	= AccountDatabaseEx.GetAccount("dropbox");
			
            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ComsumerSecret = DropBoxSecret.Password;

            // open the dropbox connection
			CloudStorage cloudStorage = new CloudStorage();
			
			DropBox.DropBoxConfiguration dpConfig = DropBox.DropBoxConfiguration.GetStandardConfiguration();
			
            cloudStorage.Open(dpConfig, cred);
			
			CloudStorage = cloudStorage;
		}
		
		[TestFixtureTearDown()]
		public void TearDownFixture()
		{
			if ( CloudStorage.IsOpened )
				CloudStorage.Close();
		}
	}
}

