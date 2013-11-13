﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    public abstract partial class GenericReferenceTests : ReferenceBaseTest 
    {        
        [Test()]
        public void AuthorizationLoginLogOffTest()
        {            
            // open the cloudstorage connection
            CloudStorage cloudStorage = new CloudStorage();
           
            // will thrown an exception
            cloudStorage.Open(Configuration, Credentials);

            // close the connection
            cloudStorage.Close();

            // check the result
            Assert.IsFalse(cloudStorage.IsOpened, "Cloudstorage logoff failed");
        }

        [Test()]
        public void AuthorizationLoginLogOffTest20Times()
        {
            for (int i = 0; i < 20; i++)
                AuthorizationLoginLogOffTest();
        }

        [Test()]
        public void AuthorizationMultiLoginTest()
        {            
            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage();
            storage1.Open(Configuration, Credentials);

            // open the dropbox connection 2
            CloudStorage storage2 = new CloudStorage();
            storage2.Open(Configuration, Credentials);

            // close the conn
            if (storage1.IsOpened)
                storage1.Close();

            if (storage2.IsOpened)
                storage2.Close();
        }

        [Test()]
        public void AuthorizationWrongPasswordTest()
        {           
            // currently not support for CIFS local
            if (Configuration is CIFSConfiguration && !Configuration.ServiceLocator.IsUnc)
                return;

            // open the dropbox connection
            Boolean bGotException = false;
            CloudStorage storage1 = new CloudStorage();

            // check throw exceotion
            try
            {
                storage1.Open(Configuration, InvalidCredentials);
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
        public void AuthorizationTokenAccessTest()
        {
            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage();
            ICloudStorageAccessToken token = storage1.Open(Configuration, Credentials);
            storage1.Close();
                        
            // open with token
            CloudStorage storage2 = new CloudStorage();            
            storage2.Open(Configuration, token);

            // read out the root
            ICloudDirectoryEntry root = storage2.GetRoot();
            Assert.IsNotNull(root);
            Assert.Greater(root.Count, 0);
            storage2.Close();
        }

        [Test()]
        public void AuthorizationTokenStoreTest()
        {           
            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage();
            ICloudStorageAccessToken token = storage1.Open(Configuration, Credentials);

            // store the token in a stream
            Stream tokenStream = storage1.SerializeSecurityToken(token);

            // close connection
            storage1.Close();

            // generate token from stream
            CloudStorage storage2 = new CloudStorage();

            // read the token
            ICloudStorageAccessToken token2 = storage2.DeserializeSecurityToken(tokenStream, Configuration);

            // use the token
            storage2.Open(Configuration, token2);

            // read out the root
            ICloudDirectoryEntry root = storage2.GetRoot();
            Assert.IsNotNull(root);
            Assert.Greater(root.Count, 0);

            storage2.Close();
        }

        [Test()]
        public void AuthorizationCheckLimits()
        {            
            // check limit
            Assert.IsNotNull(Configuration.Limits, "The limits attribute is null");           
        }
    }
}
