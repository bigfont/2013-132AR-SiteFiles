using System;
using System.Diagnostics;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.OAuth;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
    public class DropBoxBaseTest : OAuthTraceableTest
	{
		protected CloudStorage CloudStorage;
		
		public DropBoxBaseTest ()
		{
		}
		
		[TestFixtureSetUpAttribute]
		public void SetupBaseFixture()
		{                 
            // setup base
            base.SetupFixture();

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
			
            cloudStorage.Open(dpConfig, cred);
			
			CloudStorage = cloudStorage;
		}        
		
		[TestFixtureTearDown()]
		public void TearDownBaseFixture()
		{
			if ( CloudStorage.IsOpened )
				CloudStorage.Close();   
         
            // teardown base
            base.TearDownFixture();
        }        
	}
}

