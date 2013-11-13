using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.Common.IO;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{    
    partial class GenericReferenceTests : ReferenceBaseTest
    {
        public const String folderName = "test_F1";        
        public const String folderNameRenamed = "test_F1_Renamed";
        public const String folderName2 = "test_F2";        
        public const String subFolderName = "test_SF1";
        public const String newFolderName = "test_SF1_renamed";
        public const String specialCharFolder = "test_SSF {MQ R&D 50%}";
        
        [Test()]
        public void FolderCreateMoveRemoveFolder()
        {
            // 0. get the folder root             
            Assert.IsNotNull(testRoot, "Couldn't find test folder");
            
            // 1. create a folder in root directory
            ICloudDirectoryEntry newFolder = cloudStorage.CreateFolder(folderName, testRoot);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in test root");

            // 3. create a subfolder in created diretory 
            ICloudDirectoryEntry subFolder = cloudStorage.CreateFolder(subFolderName, newFolder);
            Assert.IsNotNull(subFolder, "Couldn't create the folder");

            // 4. create a second folder in root directory
            ICloudDirectoryEntry newFolder2 = cloudStorage.CreateFolder(folderName2, testRoot);
            Assert.IsNotNull(newFolder2, "Couldn't create folder with name " + folderName2 + " in root");
      
			// 5. Load the test root folder            
            Assert.IsNotNull(testRoot);

            // 6. Load the src folder
            var srcFolder = testRoot.GetChild(folderName);
            Assert.IsNotNull(srcFolder);

            // 7. Load the target folder
            var trgFolder = testRoot.GetChild(folderName2);
            Assert.IsNotNull(trgFolder);

            // 8. move the folder
            Assert.IsTrue(cloudStorage.MoveFileSystemEntry(srcFolder, trgFolder as ICloudDirectoryEntry));
			
            // 7. Load the src folder
            ICloudDirectoryEntry tgtFolder = testRoot.GetChild(folderName2) as ICloudDirectoryEntry;
            Assert.IsNotNull(tgtFolder);

            // 8. Check if former folder in the new folder
            var tgtChildfolder = tgtFolder.GetChild(folderName);
            Assert.IsNotNull(tgtChildfolder);
        }

        [Test()]
        public void FolderDoubleCreateFolder()
        {
            // 1. create a folder in root directory            
            Assert.IsNotNull(testRoot, "Couldn't find test root folder");

            // 2. create subfolder
            Assert.IsNotNull(cloudStorage.CreateFolder("TEST", testRoot));

            // 3. create subfolder again
            Assert.IsNotNull(cloudStorage.CreateFolder("TEST", testRoot));

            // 4. check the child count 			
            Assert.AreEqual(1, testRoot.Count);            
        }        

        [Test()]
        public void FolderGetPathBasedAccess()
        {            
            // 1. create a sub folder
            ICloudDirectoryEntry testFolder = cloudStorage.CreateFolder("GetPathBasedAccess", testRoot);
            Assert.IsNotNull(testFolder, "Couldn't create test folder GetPathBasedAccess");

            // 2. get the path of test root
            String path = cloudStorage.GetFileSystemObjectPath(testFolder);
            Assert.IsNotEmpty(path);

            // 3. Access folder via Path            
            Assert.IsNotNull(cloudStorage.GetFolder(path));

            // 4. remove created folder
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(testFolder));
        }

        [Test()]
        public void FolderCreateAndReadEncodedFileNames()
        {
            // 1. create the folder in test root
            ICloudDirectoryEntry newFolder = cloudStorage.CreateFolder(specialCharFolder, testRoot);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in test root");

            // 2. create a subfolder in created diretory 
            ICloudDirectoryEntry subFolder = cloudStorage.CreateFolder(subFolderName, newFolder);
            Assert.IsNotNull(subFolder, "Couldn't create the folder");

            // 3. open the folder      
            String path = cloudStorage.GetFileSystemObjectPath(newFolder);
            ICloudDirectoryEntry entry = cloudStorage.GetFolder(path);
            Assert.AreEqual(1, entry.Count);

            // 4. remove created folder
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(newFolder));            
        }

        [Test()]
        public void FolderCreateLongFolderPath()
        {
            String[] uuid = new String[4];

            uuid[0] = "aaa";
            uuid[1] = "bbb";
            uuid[2] = "ccc";
            uuid[3] = "ddd";
            
            // 1. get the path of testroot
            String path = cloudStorage.GetFileSystemObjectPath(testRoot);
            Assert.IsNotEmpty(path);

            // 2. build the long path string
            String longPath = PathHelper.Combine(path, uuid[0] + "/" + uuid[1] + "/" + uuid[2] + "/" + uuid[3]);
            Assert.IsNotEmpty(longPath);
                        
            // 3. create the long folder
            ICloudDirectoryEntry newFolder = cloudStorage.CreateFolder(longPath);                       
            Assert.IsNotNull(newFolder);

            // 4. check and remove
            for (int idx = 3; idx >= 0; idx--)
            {
                // verify the right folder
                Assert.AreEqual(uuid[idx], newFolder.Name);

                // save the parent point
                ICloudDirectoryEntry parent = newFolder.Parent;

                // remove the folder
                Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(newFolder));

                // assign the parent
                newFolder = parent;
            }
        }

        [Test()]
        public void FolderCreateUmlautPath()
        {
            // 1. get the path of testroot
            String path = cloudStorage.GetFileSystemObjectPath(testRoot);
            Assert.IsNotEmpty(path);

            // 2. build the long path string
            String longPath = PathHelper.Combine(path, "Glück/Новая папка");
            Assert.IsNotEmpty(longPath);

            // 3. create the long folder
            ICloudDirectoryEntry newFolder = cloudStorage.CreateFolder(longPath);
            Assert.IsNotNull(newFolder);

            // 4. remove folder
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(newFolder));

        }

        [Test()]
        public void FolderRenameFolderTest()
        {
            // 1. create a folder in test root directory
            ICloudDirectoryEntry newFolder = cloudStorage.CreateFolder(folderName, testRoot);
            Assert.IsNotNull(newFolder, "Couldn't create folder with name " + folderName + " in test root");

            // 2. Rename the folder
            Assert.IsTrue(cloudStorage.RenameFileSystemEntry(newFolder, folderNameRenamed));

            // 3. CHeck the change of internal data structure
            Assert.AreEqual(newFolder.Name, folderNameRenamed);

            // 4. Verify the new name of the folder
            string path = PathHelper.Combine(cloudStorage.GetFileSystemObjectPath(testRoot), folderNameRenamed);
            ICloudDirectoryEntry renamedFolder = cloudStorage.GetFolder(path, null);
            Assert.IsNotNull(newFolder, "Couldn't find renamed folder " + folderNameRenamed + " in test root");

            // 5. remove folder
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(renamedFolder));
        }      

        [Test()]
        public void FolderGetFolderWithTrailingSlash()
        {
            // get path of test root
            String path = cloudStorage.GetFileSystemObjectPath(testRoot);
            Assert.IsNotEmpty(path);

            // add trailing /
            path += "/";

            // try to get this path
            ICloudDirectoryEntry folder = cloudStorage.GetFolder(path);            
            Assert.IsNotNull(folder);
        }

        [Test()]
        public void FolderGetLongFolder()
        {
            // build path into server data root
            String path = PathHelper.Combine(testBaseFolderServerData, "/1/2/3/4/5");

            // try to get connect
            ICloudDirectoryEntry folder = cloudStorage.GetFolder(path);
            Assert.IsNotNull(folder);
        }

        [Test()]
        public void FolderGetLongFolderMissing()
        {
            // build path into server data root
            String path = PathHelper.Combine(testBaseFolderServerData, "/1/2/3/4/5/" + Guid.NewGuid().ToString());

            // try to get connect
            ICloudDirectoryEntry folder = cloudStorage.GetFolder(path, false);
            Assert.IsNull(folder);
        }

        [Test()]
        public void FolderGetLongFolderByParent()
        {
            // build path into server data root
            String path = PathHelper.Combine(testBaseFolderServerData, "/1/2/3");

            // try to get access
            ICloudDirectoryEntry folder = cloudStorage.GetFolder(path);
            Assert.IsNotNull(folder);

            // try to get the childs
            ICloudDirectoryEntry folder2 = cloudStorage.GetFolder("4/5", folder);
            Assert.IsNotNull(folder2);
        }

        [Test()]
        public void FolderTryToGetFolderAsFile()
        {
            Boolean bGotexception = false;

            try
            {
                cloudStorage.GetFile(testBaseFolderServerData, null);
            }
            catch (SharpBoxException e)
            {
                if (e.ErrorCode == SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName)
                    bGotexception = true;
            }

            Assert.IsTrue(bGotexception, "Missing ErrorInvalidFileOrDirectoryName exception");            
        }

        private int EnumerateChilds(ICloudDirectoryEntry entry)
        {
            int iChildCount = 0;

            foreach (ICloudDirectoryEntry child in entry)
            {
                iChildCount++;

                iChildCount += EnumerateChilds(child);

                Assert.IsTrue(child != entry);
            }

            return iChildCount;
        }

        [Test()]
        public void FolderChildEnumerationTest()
        {
            // build path into server data root
            String path = PathHelper.Combine(testBaseFolderServerData, "/1");

            // get the public/1 folder
            ICloudDirectoryEntry p1 = cloudStorage.GetFolder(path);

            // enumerate through all 
            int iCount = EnumerateChilds(p1);

            // verify
            Assert.AreEqual(4, iCount);
        }

        [Test()]
        public void FolderRootEnumNumberFolderTest()
        {
            // 1. create the name of our numbered folder
            String fName = "1234";
            String fileName = "12345";

            // 2. create the folder in the root
            ICloudDirectoryEntry root = cloudStorage.GetRoot();
            ICloudDirectoryEntry fr = cloudStorage.CreateFolder(fName, root);
            Assert.IsNotNull(fr);

            // 3. create the file in the root
            ICloudFileSystemEntry fs = cloudStorage.CreateFile(root, fileName);
            Assert.IsNotNull(fs);
            fs.GetDataTransferAccessor().Transfer(new MemoryStream(1), nTransferDirection.nUpload, null, null);

            // 4. enumerate over the folder
            ICloudDirectoryEntry r2 = cloudStorage.GetRoot();
            foreach (ICloudFileSystemEntry fr2 in r2)
            {
                Console.WriteLine("Enum: {0}", fr2.Name);
                Assert.AreNotEqual(0, fr2.Name.Length);                
            }

            // 5. get the created folder
            ICloudDirectoryEntry f = cloudStorage.GetFolder(fName, root);
            Assert.IsNotNull(f);

            // 6. remove the folder
            cloudStorage.DeleteFileSystemEntry(f);

            // 7. get the file
            ICloudFileSystemEntry fi = cloudStorage.GetFile(fileName, root);
            Assert.IsNotNull(fi);

            // 8. remove the ile
            cloudStorage.DeleteFileSystemEntry(fi);
        }
    }
}
