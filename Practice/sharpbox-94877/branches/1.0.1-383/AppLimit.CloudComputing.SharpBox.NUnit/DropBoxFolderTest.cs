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
        public const String folderNameRenamed = "test_{A96B7421-FFF6-4f47-BA4D-2234EAD39328}_Renamed";
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

        [Test()]
        public void CreateLongFolderPath()
        {
            String[] uuid = new String[4];
            
            uuid[0] = Guid.NewGuid().ToString();
            uuid[1] = Guid.NewGuid().ToString();
            uuid[2] = Guid.NewGuid().ToString();
            uuid[3] = Guid.NewGuid().ToString();

            // create the folder
            ICloudDirectoryEntry newFolder = CloudStorage.CreateFolder("/Public/" + uuid[0] + "/" + uuid[1] + "/" + uuid[2] + "/" + uuid[3]);
            
            // verify if a folder was created
            Assert.IsNotNull(newFolder);

            // check and remove
            for (int idx = 3; idx >= 0; idx--)
            {
                // verify the right folder
                Assert.AreEqual(uuid[idx], newFolder.Name);

                // save the parent point
                ICloudDirectoryEntry parent = newFolder.Parent;

                // remove the folder
                Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(newFolder));

                // assign the parent
                newFolder = parent;
            }
        }

        [Test()]
        public void RenameFolderTest()
        {
            // 1. create a folder in root directory
            ICloudDirectoryEntry newFolder = CloudStorage.CreateFolder(folderName, null);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in root");

            // 2. Rename the folder
            Assert.IsTrue(CloudStorage.RenameFileSystemEntry(newFolder, folderNameRenamed));

            // 3. CHeck the change of internal data structure
            Assert.AreEqual(newFolder.Name, folderNameRenamed);

            // 4. Verify the new name of the folder
            ICloudDirectoryEntry renamedFolder = CloudStorage.GetFolder("/" + folderNameRenamed, null);
            Assert.IsNotNull(newFolder, "Couldn't find renamed folder " + folderNameRenamed + " in root");

            // 5. remove folder
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(renamedFolder));
        }

        [Test()]
        public void RenameFileTest()
        {
            // 1. get public folder
            ICloudDirectoryEntry publicFolder = CloudStorage.GetFolder("/Public");

            // 2. create the upload file
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Assert.IsTrue(CloudStorage.UploadFile("C:\\windows\\notepad.exe", publicFolder));
            else
                Assert.IsTrue(CloudStorage.UploadFile("/usr/bin/which", publicFolder));
            
            // 3. Get file object
            ICloudFileSystemEntry fsEntry = CloudStorage.GetFile("/Public/notepad.exe", null );
            Assert.IsNotNull(fsEntry);

            // 4. Rename the file
            Assert.IsTrue(CloudStorage.RenameFileSystemEntry(fsEntry, "notepadRenamed.exe"));

            // 5. CHeck the change of internal data structure
            Assert.AreEqual(fsEntry.Name, "notepadRenamed.exe");

            // 6. Verify the new name of the file
            ICloudFileSystemEntry renamedFile = CloudStorage.GetFile("/Public/notepadRenamed.exe", null);
            Assert.IsNotNull(renamedFile);

            // 7. remove file
            Assert.IsTrue(CloudStorage.DeleteFileSystemEntry(renamedFile));
        }
    }
}
