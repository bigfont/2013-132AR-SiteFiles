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
    /// Summary description for DropBoxFolderTest
    /// </summary>
    [TestClass]
    public class DropBoxFolderTest : DropBoxTestItem
    {
        public const String folderName = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}";
        public const String folderName2 = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_2";
        public const String subFolderName = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_subfolder";
        public const String newFolderName = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_renamed";
        public const String doubleCreateRoot = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_double";

        public DropBoxFolderTest()
        {                        
            //
            // TODO: Add constructor logic here
            //
        }

        [Priority(3), TestMethod]
        public void CreateFolder()
        {
            // 1. create a folder in root directory
            ICloudDirectoryEntry newFolder = CloudStorage.CreateFolder(folderName, null);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in root");

            // 2. create a subfolder in created diretory 
            ICloudDirectoryEntry subFolder = CloudStorage.CreateFolder(subFolderName, newFolder);
            Assert.IsNotNull(subFolder, "Couldn't create the folder");

            // 3. create a second folder in root directory
            ICloudDirectoryEntry newFolder2 = CloudStorage.CreateFolder(folderName2, null);
            Assert.IsNotNull(newFolder2, "Couldn't create folder with name " + folderName2 + " in root");
        }


        [Priority(4), TestMethod]
        public void MoveFolder()
        {
            // 1. Load the root folder
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // 2. Load the src folder
            var srcFolder = root.GetChild(folderName);
            Assert.IsNotNull(srcFolder);

            // 3. Load the target folder
            var trgFolder = root.GetChild(folderName2);
            Assert.IsNotNull(trgFolder);

            // move the folder
            Assert.IsTrue(CloudStorage.MoveFileSystemEntry(srcFolder, trgFolder  as ICloudDirectoryEntry));
        }


        [Priority(5), TestMethod]
        public void DeleteFolder()
        {
            // 1. Load the root folder
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // 2. Load the src folder
            var tgtFolder = root.GetChild(folderName2);
            Assert.IsNotNull(tgtFolder);

            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(tgtFolder));
        }

        [Priority(6), TestMethod]
        public void DoubleCreateFolder()
        {
            // 1. create a folder in root directory
            ICloudDirectoryEntry newFolder = CloudStorage.CreateFolder(doubleCreateRoot, null);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in root");

            // 2. create subfolder
            Assert.IsNotNull(CloudStorage.CreateFolder("TEST", newFolder));

            // 3. create subfolder again
            Assert.IsNull(CloudStorage.CreateFolder("TEST", newFolder));

            // 4. check the child count 
            Assert.AreEqual<int>(1, newFolder.Count<ICloudFileSystemEntry>());

            // 4. remove testfolder
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(newFolder));
        }     
    }
}
