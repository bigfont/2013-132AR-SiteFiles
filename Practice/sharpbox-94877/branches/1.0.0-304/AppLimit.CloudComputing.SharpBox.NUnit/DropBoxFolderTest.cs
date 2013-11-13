using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
    /// <summary>
    /// Summary description for DropBoxFolderTest
    /// </summary>
    [TestFixture()]
    public class DropBoxFolderTest : DropBoxBaseTest
    {
        public const String folderName = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}";
        public const String folderName2 = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_2";
        public const String subFolderName = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_subfolder";
        public const String newFolderName = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_renamed";
        public const String doubleCreateRoot = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_double";
        public const String specialCharFolder = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328} MQ R&D 50%";

        public DropBoxFolderTest()
        {                        
            //
            // TODO: Add constructor logic here
            //
        }

        [Test()]
        public void CreateMoveRemoveFolder()
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
      
			// 4. Load the root folder
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // 5. Load the src folder
            var srcFolder = root.GetChild(folderName);
            Assert.IsNotNull(srcFolder);

            // 6. Load the target folder
            var trgFolder = root.GetChild(folderName2);
            Assert.IsNotNull(trgFolder);

            // move the folder
            Assert.IsTrue(CloudStorage.MoveFileSystemEntry(srcFolder, trgFolder  as ICloudDirectoryEntry));
			
            // 7. Load the src folder
            var tgtFolder = root.GetChild(folderName2);
            Assert.IsNotNull(tgtFolder);

            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(tgtFolder));
        }

        [Test()]
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
            Assert.AreEqual(1, newFolder.Count);

            // 4. remove testfolder
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(newFolder));
        }

        [Test()]
        public void GetPublicFolderAccess()
        {
            // 1. get the root folder
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root, "Couldn't get root foleder");

            // 2. get the public folder
            ICloudDirectoryEntry publicFolder = root.GetChild("Public") as ICloudDirectoryEntry;
            Assert.IsNotNull(publicFolder, "Couldn't get root foleder");

            // 3. create new folder
            ICloudDirectoryEntry subFolder = CloudStorage.CreateFolder(subFolderName, publicFolder);
            Assert.IsNotNull(subFolder, "Couldn't create object in public folder");

            // 4. remove created folder
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(subFolder));
        }

        [Test()]
        public void GetPathBasedAccess()
        {
            // 1. create a folder in root directory
            ICloudDirectoryEntry newFolder = CloudStorage.CreateFolder(folderName, null);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in root");

            // 2. create a subfolder in created diretory 
            ICloudDirectoryEntry subFolder = CloudStorage.CreateFolder(subFolderName, newFolder);
            Assert.IsNotNull(subFolder, "Couldn't create the folder");

            // 3. Access folder via Path            
            String path = "/" + folderName + "/" + subFolderName;
            Assert.IsNotNull(CloudStorage.GetFolder(path));

            // 4. remove created folder
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(newFolder));
        }

        [Test()]
        public void CreateAndReadEncodedFileNames()
        {
            // 1. create the folder
            ICloudDirectoryEntry newFolder = CloudStorage.CreateFolder(specialCharFolder, null);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in root");

            // 2. create a subfolder in created diretory 
            ICloudDirectoryEntry subFolder = CloudStorage.CreateFolder(subFolderName, newFolder);
            Assert.IsNotNull(subFolder, "Couldn't create the folder");

            // 3. open the folder            
            ICloudDirectoryEntry entry = CloudStorage.GetFolder("/" + specialCharFolder);
            Assert.AreEqual(1, entry.Count);

            // 4. remove created folder
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(newFolder));            
        }
    }
}
