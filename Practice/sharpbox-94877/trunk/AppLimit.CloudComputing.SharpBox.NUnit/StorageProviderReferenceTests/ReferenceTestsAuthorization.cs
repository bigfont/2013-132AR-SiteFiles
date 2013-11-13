using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    public abstract partial class GenericReferenceTests : ReferenceBaseTest 
    {        
        [Test()]
        public void AuthorizationLoginLogOffTest()
        {            
            // open the cloudstorage connection
            CloudStorage newCloudStorage = new CloudStorage(this.cloudStorage, false);
           
            // will thrown an exception
            newCloudStorage.Open(Configuration, ValidAccessToken);

            // close the connection
            newCloudStorage.Close();

            // check the result
            Assert.IsFalse(newCloudStorage.IsOpened, "Cloudstorage logoff failed");
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
            CloudStorage storage1 = new CloudStorage(this.cloudStorage, false);
            storage1.Open(Configuration, ValidAccessToken);

            // open the dropbox connection 2
            CloudStorage storage2 = new CloudStorage(this.cloudStorage, false);
            storage2.Open(Configuration, ValidAccessToken);

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
            CloudStorage storage1 = new CloudStorage(this.cloudStorage, false);

            // check throw exceotion
            try
            {
                storage1.Open(Configuration, InvalidAccessToken);
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
            CloudStorage storage1 = new CloudStorage(this.cloudStorage, false);
            ICloudStorageAccessToken token = storage1.Open(Configuration, ValidAccessToken);
            storage1.Close();
                        
            // open with token
            CloudStorage storage2 = new CloudStorage(this.cloudStorage);            
            storage2.Open(Configuration, token);

            // read out the root
            ICloudDirectoryEntry root = storage2.GetRoot();
            Assert.IsNotNull(root);
            Assert.Greater(root.Count, 0);
            storage2.Close();
        }

        [Test()]
        public void AuthorizationTokenAccessBae64Test()
        {
            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage(this.cloudStorage, false);
            ICloudStorageAccessToken token = storage1.Open(Configuration, ValidAccessToken);
            storage1.Close();

            // convert to base64
            String base64Token = storage1.SerializeSecurityTokenToBase64Ex(token, storage1.CurrentConfiguration.GetType(), null);
            Assert.IsNotEmpty(base64Token);

            // deserialize token 
            ICloudStorageAccessToken tokenNew =  storage1.DeserializeSecurityTokenFromBase64(base64Token);
            Assert.IsNotNull(tokenNew);

            // open with token
            CloudStorage storage2 = new CloudStorage(this.cloudStorage);
            storage2.Open(Configuration, tokenNew);

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
            CloudStorage storage1 = new CloudStorage(this.cloudStorage, false);
            ICloudStorageAccessToken token = storage1.Open(Configuration, ValidAccessToken);

            // store the token in a stream
            Stream tokenStream = storage1.SerializeSecurityToken(token);            

            // close connection
            storage1.Close();

            // generate token from stream
            CloudStorage storage2 = new CloudStorage(this.cloudStorage, false);

            // read the token
            ICloudStorageAccessToken token2 = storage2.DeserializeSecurityToken(tokenStream);

            // use the token
            storage2.Open(Configuration, token2);

            // read out the root
            ICloudDirectoryEntry root = storage2.GetRoot();
            Assert.IsNotNull(root);
            Assert.Greater(root.Count, 0);

            storage2.Close();
        }

        [Test()]
        public void AuthorizationTokenStoreWithMetadataTest()
        {
            // open the dropbox connection 1            
            CloudStorage storage1 = new CloudStorage(this.cloudStorage, false);
            ICloudStorageAccessToken token = storage1.Open(Configuration, ValidAccessToken);

            // metadata
            Dictionary<String, String> d = new Dictionary<string, string>();
            d.Add("Meta1", "Hello World");
            d.Add("Meta2", "Hello Universe");

            // store the token in a stream
            Stream tokenStream = storage1.SerializeSecurityToken(token, d);

            // close connection
            storage1.Close();

            // generate token from stream
            CloudStorage storage2 = new CloudStorage(this.cloudStorage, false);

            // result meta
            Dictionary<String, String> r = null;

            // read the token
            ICloudStorageAccessToken token2 = storage2.DeserializeSecurityToken(tokenStream,out r);

            // check the meta data
            Assert.IsNotNull(r);
            Assert.That(r.Count == 2);
            Assert.That(r.ContainsKey("Meta1"));
            Assert.That(r.ContainsKey("Meta2"));

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
