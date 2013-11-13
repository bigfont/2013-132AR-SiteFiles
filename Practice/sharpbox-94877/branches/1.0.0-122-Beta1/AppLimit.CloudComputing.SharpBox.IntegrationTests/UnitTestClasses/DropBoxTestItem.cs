using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.IntegrationTests.UnitTestClasses
{
    public class DropBoxTestItem : SharpBoxTestItem
    {
        public AccountDatabase DropBoxSecret { get; private set; }

        public AccountDatabase UserAccount { get; private set; }

        public DropBoxTestItem()
        {
            // read the account
            DropBoxSecret = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp");
            UserAccount = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropbox");
            
            // build the credentials for dropbox
            DropBox.DropBoxCredentials cred = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            cred.UserName = UserAccount.User;
            cred.Password = UserAccount.Password;
            cred.ConsumerKey = DropBoxSecret.User;
            cred.ComsumerSecret = DropBoxSecret.Password;

            // open the dropbox connection
            Boolean bret = CloudStorage.Open(DropBox.DropBoxConfiguration.GetStandardConfiguration(), cred);

            // check the result 
            Assert.IsTrue(bret, "Dropbox login failed");                
        }     
    }
}
