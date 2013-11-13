using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Threading;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common.IO;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav;
using AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    partial class GenericReferenceTests : ReferenceBaseTest 
    {
        [Serializable]
        public class SerializableClassForTest
        {
            public String strValue;
            public int iValue;
            public float fValue;
            public SerializableClassForTest refValue;
        }

        private const string SerializableFileName = "upload.ser";
        private const String SerializableTestStrValue = "Hello World";
        private const int SerializableTestiValue = 99;
        private const float SerializableTestfValue = 107.98F;        

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch ;
            for(int i=0; i<size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))) ;
                builder.Append(ch); 
            }
            if(lowerCase)
                return builder.ToString().ToLower();
            
            return builder.ToString();
        }


        [Test()]
        public void FileCreateFileWithRandomData()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = cloudStorage.CreateFile(testRoot, "upload.stream");            
            Assert.IsNotNull(newFile);

            // content stream
            Stream dataStream = new MemoryStream();

            // open a textwrite            
            using(StreamWriter swriter = new StreamWriter(dataStream))
            {
                // upload random stuff
                for (int i = 0; i < 100; i++)
                {
                    swriter.WriteLine(RandomString(130, true));
                }

                // go to start
                dataStream.Position = 0;

                // transfer the data
                newFile.GetDataTransferAccessor().Transfer(dataStream, nTransferDirection.nUpload, TestFileOperationProgressChanged, null);
            }
           
            // remove the file 
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(newFile));
        }

        private ICloudFileSystemEntry FileUploadLargeFileWithRandomData(long sizeinMByte)
        {
            // create the upload file
            ICloudFileSystemEntry newFile = cloudStorage.CreateFile(testRoot, "uploadLarge" + sizeinMByte.ToString() + ".stream");
            Assert.IsNotNull(newFile);

            // content stream            
            using (Stream dataStream = new MemoryStream())
            {
                // upload random stuff
                for (int i = 0; i < sizeinMByte; i++)
                {
                    dataStream.Write(new byte[1024 * 1024], 0, 1024 * 1024);
                    Console.Write(".");

                    if (i % 100 == 0)
                        Console.WriteLine("");
                }

                Console.WriteLine("");

                // go to start
                dataStream.Position = 0;

                // transfer the data
                newFile.GetDataTransferAccessor().Transfer(dataStream, nTransferDirection.nUpload, TestFileOperationProgressChanged, null);
            }

            return newFile;
        }

        [Test()]
        public void FileUploadToLargeFileWithRandomData()
        {
            // if we have no upload limit, just set one
            int iLimit = Configuration.Limits.MaxUploadFileSize == -1 ? 500 : Configuration.Limits.MaxUploadFileSize;

            SharpBoxException exc = null;

            // generate and upload this file
            try
            {
                FileUploadLargeFileWithRandomData(iLimit / 1024 / 1024 + 2);
            }
            catch (SharpBoxException e)
            {
                exc = e;
            }

            // verify the error condition
            if (Configuration.Limits.MaxUploadFileSize == -1)
                Assert.IsNull(exc, "A limit was exceeded where no limit was defined");
            else
                Assert.IsNotNull(exc, "No limit was exceeded");        
        }

        [Test()]
        [Category("ExcludedOnDeveloperMachine")]
        public void FileUploadLargeFileWithRandomData()
        {            
            ICloudFileSystemEntry entry = FileUploadLargeFileWithRandomData(Configuration.Limits.MaxUploadFileSize / 1024 / 1024 - 30);

            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(entry));
        }

        [Test()]
        public void FileUploadBinaryTest()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = cloudStorage.CreateFile(testRoot, "notepad.exe");
            Assert.IsNotNull(newFile);

            // open file stream
            FileStream file = null;
			
			if ( Environment.OSVersion.Platform == PlatformID.Win32NT)
				file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);
			else
				file = new FileStream("/usr/bin/which", FileMode.Open, FileAccess.Read);

            
            // transfer data
            newFile.GetDataTransferAccessor().Transfer(file, nTransferDirection.nUpload, null, null);
                        
            // remove the file 
            cloudStorage.DeleteFileSystemEntry(newFile);
        }

        [Test()]
        public void FileUploadBinaryCCTest()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = cloudStorage.CreateFile(testRoot, "notepad,,.exe");
            Assert.IsNotNull(newFile);

            // get file steam
            FileStream file = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);
            else
                file = new FileStream("/usr/bin/which", FileMode.Open, FileAccess.Read);

            // upload data
            newFile.GetDataTransferAccessor().Transfer(file, nTransferDirection.nUpload, null, null);
            Assert.AreEqual(newFile.Length, file.Length);

            // remove the file 
            cloudStorage.DeleteFileSystemEntry(newFile);
        }

        [Test()]
        public void FileUploadComfortRename()
        {           
            // upload a file
            ICloudFileSystemEntry fEntry = null;
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                fEntry = cloudStorage.UploadFile("C:\\windows\\notepad.exe", testRoot, "newNamedFile");
            else
                fEntry = cloudStorage.UploadFile("/usr/bin/which", testRoot, "newNamedFile");
            
            // check
            Assert.AreEqual(fEntry.Name, "newNamedFile");
            
            // remove the file 
            cloudStorage.DeleteFileSystemEntry(fEntry);
        }        

        [Test()]
        public void FileUploadBinaryViaPathTest()
        {
            // get the root path
            string path = cloudStorage.GetFileSystemObjectPath(testRoot);

            // get public filder
            ICloudDirectoryEntry publicFolder = cloudStorage.GetFolder(path);

            // create the upload file
            ICloudFileSystemEntry newFile = cloudStorage.CreateFile(publicFolder, "notepad.exe");
            Assert.IsNotNull(newFile);

            // get file stream
            FileStream file = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);
            else
                file = new FileStream("/usr/bin/which", FileMode.Open, FileAccess.Read);

            // upload data
            newFile.GetDataTransferAccessor().Transfer(file, nTransferDirection.nUpload, null, null);
            
            // remove the file 
            cloudStorage.DeleteFileSystemEntry(newFile);
        }

        [Test()]
        public void FileUploadBinaryComfortTest()
        {
            // set the source file
            String filePath;
            if ( Environment.OSVersion.Platform == PlatformID.Win32NT)
				filePath = "C:\\windows\\notepad.exe";
			else
				filePath = "/usr/bin/which";


            // get the root path
            string path = cloudStorage.GetFileSystemObjectPath(testRoot);

            // get the public folder
            ICloudDirectoryEntry publicFolder = cloudStorage.GetFolder(path);
            Assert.IsNotNull(publicFolder, "Couldn't get public folder");

            // upload the file 
            Assert.IsNotNull(cloudStorage.UploadFile(filePath, publicFolder));

            // upload the file 
            Assert.IsNotNull(cloudStorage.UploadFile(filePath, path));
        }

        [Test()]
        public void FileDownloadGifTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(root);

            // find the right file
            ICloudFileSystemEntry readFile = root.GetChild("hhw.gif");
            Assert.IsNotNull(readFile);
            Assert.IsNotInstanceOfType(typeof(ICloudDirectoryEntry), readFile);
            
            // get the stream
            Stream data = new MemoryStream();
            Assert.IsNotNull(data);

            // Transfer data
            readFile.GetDataTransferAccessor().Transfer(data, nTransferDirection.nDownload, null, null);

            // checks
            long iReadBytes = data.Length;
            Assert.AreNotEqual(iReadBytes, 0);
            Assert.LessOrEqual(iReadBytes, readFile.Length);

            // close the stream
            data.Close();
        }

        [Test()]
        public void FileAccessSpecialCharTest1()
        {
            // open the root folder object
            ICloudDirectoryEntry publicFolder = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(publicFolder);

            // find the right file
            ICloudFileSystemEntry fe = publicFolder.GetChild("te'st.txt");
            Assert.IsNotNull(fe);
            Assert.IsNotInstanceOfType(typeof(ICloudDirectoryEntry), fe);            
        }

        [Test()]
        public void FileDownloadGifCCTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(root);

            // find the right file
            ICloudFileSystemEntry readFile = root.GetChild("hhw,,.gif");
            Assert.IsNotNull(readFile);
            Assert.IsNotInstanceOfType(typeof(ICloudDirectoryEntry), readFile);

            // get the stream
            Stream data = new MemoryStream();
            Assert.IsNotNull(data);

            // Transfer data
            readFile.GetDataTransferAccessor().Transfer(data, nTransferDirection.nDownload, null, null);
            
            // verify            
            long iReadBytes = data.Length;
            Assert.AreNotEqual(iReadBytes, 0);
            Assert.LessOrEqual(iReadBytes, readFile.Length);

            // close the stream
            data.Close();
        }

        [Test()]
        public void FileDownloadGifCCTestByStream()
        {
            // open the root folder object
            ICloudDirectoryEntry root = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(root);

            // find the right file
            ICloudFileSystemEntry readFile = root.GetChild("hhw,,.gif");
            Assert.IsNotNull(readFile);
            Assert.IsNotInstanceOfType(typeof(ICloudDirectoryEntry), readFile);

            // save data
            Byte[] data = new Byte[readFile.Length];

            // Transfer data
            using (Stream input = readFile.GetDataTransferAccessor().GetDownloadStream())
            {
                Assert.AreEqual(readFile.Length, input.Length);
                input.Read(data, 0, Convert.ToInt32(readFile.Length));
            }

            // verify            
            long iReadBytes = data.Length;
            Assert.AreNotEqual(iReadBytes, 0);
            Assert.LessOrEqual(iReadBytes, readFile.Length);            
        }

        void TestFileOperationProgressChanged(object sender, FileDataTransferEventArgs e)
        {                        
            if (e.TotalBytes == 0)
                Console.WriteLine("[{2}] Progress: {1} {0} Bytes (CRate: {3} KBit/s TRate: {4} KBit/s F: {5}s)", e.CurrentBytes, DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, e.TransferRateCurrent, e.TransferRateTotal, e.OpenTransferTime.TotalSeconds);
            else
                Console.WriteLine("[{4}] Progress: {3} {0} of {1} Bytes ({2}%, CRate: {5} KBit/s TRate: {6} KBit/s F: {7}s)", e.CurrentBytes, e.TotalBytes, e.PercentageProgress, DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, e.TransferRateCurrent, e.TransferRateTotal, e.OpenTransferTime.TotalSeconds);
            
            e.Cancel = false;
        }

        void TestFileOperationProgressChangedAbort(object sender, FileDataTransferEventArgs e)
        {
            e.Cancel = true;
        }

        [Test()]
        public void FileDownloadGifComfortTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(root);

            // download the file
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                cloudStorage.DownloadFile(root, "hhw.gif", "%temp%");
			else
                cloudStorage.DownloadFile(root, "hhw.gif", "/tmp");

            // download the file with second methid
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                cloudStorage.DownloadFile(testBaseFolderServerData + "/hhw.gif", "%temp%");
			else
                cloudStorage.DownloadFile(testBaseFolderServerData + "/hhw.gif", "/tmp");

            // download the file with third method
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                cloudStorage.DownloadFile(root, "hhw.gif", "%temp%", TestFileOperationProgressChanged);
            else
                cloudStorage.DownloadFile(root, "hhw.gif", "/tmp", TestFileOperationProgressChanged);
        }

        [Test()]
        public void FileDownloadGifComfortAbortedTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(root);

            SharpBoxException neededE = null;

            try
            {
                // download the file with abortion
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    cloudStorage.DownloadFile(root, "hhw.gif", "%temp%", TestFileOperationProgressChangedAbort);
                else
                    cloudStorage.DownloadFile(root, "hhw.gif", "/tmp", TestFileOperationProgressChangedAbort);
            }
            catch (SharpBoxException e)
            {
                neededE = e;
            }

            Assert.IsNotNull(neededE);
            Assert.AreEqual(neededE.ErrorCode, SharpBoxErrorCodes.ErrorTransferAbortedManually);
        }

        [Test()]
        public void FileDownloadNotExistingFile()
        {
            // open the root folder object
            ICloudDirectoryEntry root = cloudStorage.GetFolder(testBaseFolderServerData);
            Assert.IsNotNull(root);

            Boolean bGotException = false;

            try
            {
                // download the file
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    cloudStorage.DownloadFile(root, "/MissingFile.missing", "%temp%");
                else
                    cloudStorage.DownloadFile(root, "/MissingFile.missing", "/tmp");
            }
            catch (SharpBoxException e)
            {
                if (e.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
                    bGotException = true;
            }

            Assert.IsTrue(bGotException);
        }

        [Test()]
        public void FileSerializableTest()
        {
            // open the root folder object            
            Assert.IsNotNull(testRoot);            

            { // write stuff
                // create the upload file
                ICloudFileSystemEntry newFile = cloudStorage.CreateFile(testRoot, SerializableFileName);
                Assert.IsNotNull(newFile);
                
                // build the object
                SerializableClassForTest sample = new SerializableClassForTest();
                sample.strValue = SerializableTestStrValue;
                sample.iValue = SerializableTestiValue;
                sample.fValue = SerializableTestfValue;
                sample.refValue = new SerializableClassForTest();
                sample.refValue.strValue = SerializableTestStrValue;
                sample.refValue.iValue = SerializableTestiValue;
                sample.refValue.fValue = SerializableTestfValue;

                // serialize with formatter
                BinaryFormatter bformatter = new BinaryFormatter();
                newFile.GetDataTransferAccessor().Serialize(bformatter, sample);
            }

            {   // read stuff

                // read the file
                ICloudFileSystemEntry readFile = testRoot.GetChild(SerializableFileName);
                Assert.IsNotNull(readFile);
            
                // deserialize with formatter
                BinaryFormatter bformatter = new BinaryFormatter();
                SerializableClassForTest obj = readFile.GetDataTransferAccessor().Deserialize(bformatter) as SerializableClassForTest;
                Assert.IsNotNull(obj);

                Assert.AreEqual(obj.strValue, SerializableTestStrValue);
                Assert.AreEqual(obj.iValue, SerializableTestiValue);
                Assert.AreEqual(obj.fValue, SerializableTestfValue);

                Assert.IsNotNull(obj.refValue);                

                Assert.AreEqual(obj.refValue.strValue, SerializableTestStrValue);
                Assert.AreEqual(obj.refValue.iValue, SerializableTestiValue);
                Assert.AreEqual(obj.refValue.fValue, SerializableTestfValue);

                // remove the file 
                cloudStorage.DeleteFileSystemEntry(readFile);
            }			   			
        }
        
        public void FileUploadFileRoutine(String fileNameFromClientData, Boolean bInterrupManually, Boolean bAsyncStatus)
        {
            // set the names
            String fileName = fileNameFromClientData;
		
            String fullLocalName = String.Empty;
			
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				fullLocalName = Environment.ExpandEnvironmentVariables("%SystemDrive%\\ClientData\\") + fileName;
			else
			{
				fullLocalName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"ClientData");				
				fullLocalName = Path.Combine(fullLocalName, fileName);
			}
			
            String fullRemotName = PathHelper.Combine(cloudStorage.GetFileSystemObjectPath(testRoot), fileName);
            
            // upload file
            if (bAsyncStatus)
            {
                // create file
                ICloudFileSystemEntry fe = cloudStorage.CreateFile(testRoot, fileName);

                // ope th local file
                using(FileStream fs = File.OpenRead(fullLocalName))
                {
                    // get teh transfer
                    fe.GetDataTransferAccessor().TransferAsyncProgress(fs, nTransferDirection.nUpload, TestFileOperationProgressChanged, null);
                }                    
            }
            else if (bInterrupManually)
            {
                SharpBoxException neededE = null;

                try
                {
                    cloudStorage.UploadFile(fullLocalName, testRoot, TestFileOperationProgressChangedAbort);
                }
                catch (SharpBoxException e)
                {
                    neededE = e;
                }

                Assert.IsNotNull(neededE);
                Assert.AreEqual(neededE.ErrorCode, SharpBoxErrorCodes.ErrorTransferAbortedManually);
            }
            else
                cloudStorage.UploadFile(fullLocalName, testRoot, TestFileOperationProgressChanged);

            if (!bInterrupManually)
            {
                // download file
                String targetDir = String.Empty;

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    targetDir = Environment.GetEnvironmentVariable("temp");
                else
                    targetDir = "/tmp";

                cloudStorage.DownloadFile(testRoot, fileName, targetDir, TestFileOperationProgressChanged);

                // compare files
                String left = fullLocalName;
                String right = Path.Combine(targetDir, fileName);

                String md5Left = GenerateMD5Hash(left);
                String md5Right = GenerateMD5Hash(right);

                // remove files
                File.Delete(right);

                // remove file on the left side as well
                Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(fullRemotName));

                // check the hashes
                Assert.AreEqual(md5Left, md5Right);
            }
        }

        [Test()]
        public void FileUploadDifferntFiles()
        {
            FileUploadFileRoutine("DirectUploadTest.bmp", false, false);
            FileUploadFileRoutine("Glück.pdf", false, false);
        }

        [Test()]
        public void FileUploadDifferntFilesAsyncStatus()
        {
            FileUploadFileRoutine("DirectUploadTest.bmp", false, true);
            FileUploadFileRoutine("Glück.pdf", false, true);
        }

        [Test()]
        public void FileUploadABorted()
        {            
            FileUploadFileRoutine("Glück.pdf", true, false);
        }               


        private String GenerateMD5Hash(string path )
        {
            // read file
            System.IO.FileStream FileCheck = System.IO.File.OpenRead(path);

            // calc hash
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] md5Hash = md5.ComputeHash(FileCheck);
            FileCheck.Close();

            //convert hash to string
            return BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
        }

        [Test()]
        public void FileTestDirectDownloadUrl()
        {
            // open the publicFilder
            ICloudDirectoryEntry publicFolder = cloudStorage.GetFolder(testBaseFolderServerData);

            // get the direct url to the pptx
            Uri uri = cloudStorage.GetFileSystemObjectUrl("DirectDownloadTest.pptx", publicFolder);

            // target
            String temp = Environment.ExpandEnvironmentVariables("%temp%\\deg.pptx");

            // download to temp
            WebClient cl = new WebClient();

            // in the case of webdav we need to add the creds
            if (ValidAccessToken is ICredentials)
            {
                ICredentials creds = ValidAccessToken as ICredentials;
                cl.Credentials = creds.GetCredential(null, null);
            }

            cl.DownloadFile(uri, temp);

            // check if file exists
            Assert.That(File.Exists(temp));

            // remove the file
            File.Delete(temp);
        }

        [Test()]
        public void FileRenameFileTest()
        {            
            // 2. create the upload file
            String fileName;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fileName = "notepad.exe";
                Assert.IsNotNull(cloudStorage.UploadFile("C:\\windows\\" + fileName, testRoot));
            }
            else
            {
                fileName = "which";
                Assert.IsNotNull(cloudStorage.UploadFile("/usr/bin/" + fileName, testRoot));
            }

            // 3. Get file object
            ICloudFileSystemEntry fsEntry = cloudStorage.GetFile(fileName, testRoot);
            Assert.IsNotNull(fsEntry);

            // 4. Rename the file
            Assert.IsTrue(cloudStorage.RenameFileSystemEntry(fsEntry, "notepadRenamed.exe"));

            // 5. CHeck the change of internal data structure
            Assert.AreEqual(fsEntry.Name, "notepadRenamed.exe");

            // 6. Verify the new name of the file
            ICloudFileSystemEntry renamedFile = cloudStorage.GetFile("notepadRenamed.exe", testRoot);
            Assert.IsNotNull(renamedFile);

            // 7. Verify that the old file name is no more available
            ICloudFileSystemEntry oldFile = null;
            //ICloudFileSystemEntry oldFile = cloudStorage.GetFile(fileName, testRoot);
            //Assert.IsNull(oldFile);

            // 8. Recreate oldfile as empty file
            // build a data stream            
            using (MemoryStream data = new MemoryStream())
            {
                // upload file
                oldFile = cloudStorage.CreateFile(testRoot, fileName);
                Assert.IsNotNull(oldFile);

                oldFile.GetDataTransferAccessor().Transfer(data, nTransferDirection.nUpload, null, null);
            }

            // 8. remove file
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(oldFile));
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(renamedFile));
        }

        [Test()]
        public void FileRenameFileWithLostParent()
        {
            // 2. create the upload file
            String fileName;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fileName = "notepad.exe";
                Assert.IsNotNull(cloudStorage.UploadFile("C:\\windows\\" + fileName, testRoot));
            }
            else
            {
                fileName = "which";
                Assert.IsNotNull(cloudStorage.UploadFile("/usr/bin/" + fileName, testRoot));
            }

            // 3. Get file object
            ICloudFileSystemEntry fsEntry = cloudStorage.GetFile(fileName, testRoot);
            Assert.IsNotNull(fsEntry);

            // 4. Rename the file
            Assert.IsTrue(cloudStorage.RenameFileSystemEntry(fsEntry, "notepadRenamed.exe"));
                        
            // 3. create lastfile new
            ICloudFileSystemEntry oldfile = cloudStorage.CreateFile(testRoot, fileName);
            Assert.IsNotNull(oldfile.Parent);

            // check if this operation changes the parent in the renamed file
            Assert.IsNotNull(fsEntry.Parent);

            // 8. remove file            
            Assert.IsTrue(cloudStorage.DeleteFileSystemEntry(fsEntry));
        }      

        void FileOperationProgress(ICloudFileSystemEntry file, long currentbytes, long sizebytes)
        {
            Console.Write(".");
        }

        [Test()]
        public void FileMultipleDownloadTest()
        {
            // WebDAV currently does not support special properties            
            if (ValidAccessToken is GenericNetworkCredentials)
                return;                

            // build the file liste
            List<String> files = new List<string>();
            files.Add( PathHelper.Combine(testBaseFolderServerData, "/MultipleDownloads/DirectDownloadTest1.pptx"));
            files.Add( PathHelper.Combine(testBaseFolderServerData, "/MultipleDownloads/DirectDownloadTest2.pptx"));
            files.Add( PathHelper.Combine(testBaseFolderServerData, "/MultipleDownloads/DirectDownloadTest3.pptx"));
            files.Add( PathHelper.Combine(testBaseFolderServerData, "/MultipleDownloads/DirectDownloadTest4.pptx"));
            
            // we use a thread aray
            List<Thread> th = new List<Thread>();

            // start our worker
            foreach(string file in files)
            {
                Thread t1 = new Thread(FileMultipleDownloadTestThread);
                th.Add(t1);
                t1.Start(file);                
            }            

            // wait for worker
            foreach (Thread t in th)
                t.Join();

            // ok now check if we have 4 files in target dir
            String tempDir = String.Empty;
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				tempDir = Environment.ExpandEnvironmentVariables("%temp%");
			else
				tempDir = "/tmp";
			
			foreach(string file in files)
			{
            	Assert.IsTrue(File.Exists(Path.Combine(tempDir, Path.GetFileName(file))));
            	
				// clean up
				File.Delete(Path.Combine(tempDir, Path.GetFileName(file)));
			}            
        }

        void FileMultipleDownloadTestThread(object obj)
        {
            try
            {
                // build a new sendbox derived from the old one
                CloudComputing.SharpBox.CloudStorage cloudSandbox = new CloudStorage(cloudStorage);

                // build a path 
                String dlPath = String.Empty;
				
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					dlPath = Environment.ExpandEnvironmentVariables("%temp%");
				else
					dlPath = "/tmp";

                // new download the files
                cloudSandbox.DownloadFile(obj as String, dlPath, TestFileOperationProgressChanged);
            }
            catch (Exception e)
            {
                Thread.CurrentThread.Abort(e);                
            }
        }

        [Test()]
        public void FileTryToGetDeletedFile()
        {
            // 2. create the upload file
            String fileName;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fileName = "notepad.exe";
                Assert.IsNotNull(cloudStorage.UploadFile("C:\\windows\\" + fileName, testRoot));
            }
            else
            {
                fileName = "which";
                Assert.IsNotNull(cloudStorage.UploadFile("/usr/bin/" + fileName, testRoot));
            }

            // 3. Get file object
            ICloudFileSystemEntry fsEntry = cloudStorage.GetFile(fileName, testRoot);
            Assert.IsNotNull(fsEntry);

            // 4. delete file
            cloudStorage.DeleteFileSystemEntry(fsEntry);

            // 5. try to get file
            ICloudFileSystemEntry fsRemoved = cloudStorage.GetFileSystemObject(fileName, testRoot);
            Assert.IsNull(fsRemoved);
        }
    }
}
