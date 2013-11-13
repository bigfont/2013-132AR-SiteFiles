using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox.NUnit.Common;
using AppLimit.CloudComputing.SharpBox.Common.IO;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    public abstract class ReferenceBaseTest : WebRequestTraceableTest
    {
        protected ICloudStorageConfiguration Configuration;
        protected ICloudStorageConfiguration ConfigurationNonExistingServiceRoot;
        protected ICloudStorageCredentials Credentials;
        protected ICloudStorageCredentials InvalidCredentials;
        protected CloudStorage cloudStorage;

        protected const string testBaseFolder = "/NUnitIntegrationTests/Workspaces";
        protected const string testBaseFolderServerData = "/NUnitIntegrationTests/ServerData";

        protected ICloudDirectoryEntry testFictureRoot;
        protected ICloudDirectoryEntry testRoot;
       
        public abstract void InitializeProvider();        

        [TestFixtureSetUpAttribute]
        public void SetupBaseFixture()
        {
            // initialize a new cloud storage class
            cloudStorage = new CloudStorage();

            // initialize the providers
            InitializeProvider();

            // setup base
            base.SetupFixture();
           
            // open the cloud storage connection            
            cloudStorage.Open(Configuration, Credentials); 
           
            // create the testfixture root
            testFictureRoot = cloudStorage.CreateFolderEx(PathHelper.Combine(testBaseFolder, Guid.NewGuid().ToString()), null);

            // check the folder
            if (testFictureRoot == null)
                throw new Exception("Failed to create the root testfolder, check your storage provider");
        }

        [TestFixtureTearDown()]
        public void TearDownBaseFixture()
        {
            // remove the folder
            if (testFictureRoot != null)
                cloudStorage.DeleteFileSystemEntry(testFictureRoot);

            // close the cloud storage
            if (cloudStorage.IsOpened)
                cloudStorage.Close();

            // teardown base
            base.TearDownFixture();
        }

        [SetUp]
        public void SetupTestFolder()
        {
            // setup the base test
            base.SetupTest();

            // setup the test root            
            testRoot = cloudStorage.CreateFolder(Guid.NewGuid().ToString(), testFictureRoot);

            // check the folder
            if (testRoot == null)
                throw new Exception("Failed to create the root testfolder, check your storage provider");
        }

        [TearDown]
        public void TearTestFolder()
        {
            // remove the testfolder
            if (testRoot != null)
            {
                cloudStorage.DeleteFileSystemEntry(testRoot);
            }
            
            // teardown the base test
            base.TearTest();

        }
    }
}
