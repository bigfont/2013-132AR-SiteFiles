using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
    /// <summary>
    /// Summary description for DropBoxFileStreamTest
    /// </summary>
    [TestFixture()]
    public class DropBoxFileStreamTest : DropBoxBaseTest
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

        public DropBoxFileStreamTest()
        {
            //
            // TODO: Add constructor logic here
            //			
        }

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
        public void CreateFileWithRandomData()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = CloudStorage.CreateFile(null, "upload.stream");            
            Assert.IsNotNull(newFile);

            // content stream
            Stream dataStream = newFile.GetContentStream(FileAccess.Write);

            // open a textwrite
            StreamWriter swriter = new StreamWriter(dataStream);

            // upload random stuff
            for (int i = 0; i < 100; i++)
            {
                swriter.WriteLine(RandomString(130, true));
            }

            // close the writer
            swriter.Close();

            // close the data
            dataStream.Close();

            // remove the file 
            CloudStorage.DeleteFileSystemEntry(newFile);
        }

        [Test()]
        public void UploadBinaryTest()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = CloudStorage.CreateFile(null, "notepad.exe");
            Assert.IsNotNull(newFile);

            // content stream
            Stream dataStream = newFile.GetContentStream(FileAccess.Write);

            FileStream file = null;
			
			if ( Environment.OSVersion.Platform == PlatformID.Win32NT)
				file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);
			else
				file = new FileStream("/usr/bin/which", FileMode.Open, FileAccess.Read);

            Byte[] data = new Byte[file.Length];

            file.Read(data, 0, (int)file.Length);

            dataStream.Write(data, 0, (int)file.Length);
                        
            // close the data
            dataStream.Close();            

            // remove the file 
            CloudStorage.DeleteFileSystemEntry(newFile);
        }

        [Test()]
        public void UploadBinaryCCTest()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = CloudStorage.CreateFile(null, "notepad,,.exe");
            Assert.IsNotNull(newFile);

            // content stream
            Stream dataStream = newFile.GetContentStream(FileAccess.Write);

            FileStream file = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);
            else
                file = new FileStream("/usr/bin/which", FileMode.Open, FileAccess.Read);

            Byte[] data = new Byte[file.Length];

            file.Read(data, 0, (int)file.Length);

            dataStream.Write(data, 0, (int)file.Length);

            // close the data
            dataStream.Close();

            // remove the file 
            CloudStorage.DeleteFileSystemEntry(newFile);
        }


        [Test()]
        public void UploadBinaryViaPathTest()
        {
            // get public filder
            ICloudDirectoryEntry publicFolder = CloudStorage.GetFolder("/Public");

            // create the upload file
            ICloudFileSystemEntry newFile = CloudStorage.CreateFile(publicFolder, "notepad.exe");
            Assert.IsNotNull(newFile);

            // content stream
            Stream dataStream = newFile.GetContentStream(FileAccess.Write);

            FileStream file = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);
            else
                file = new FileStream("/usr/bin/which", FileMode.Open, FileAccess.Read);

            Byte[] data = new Byte[file.Length];

            file.Read(data, 0, (int)file.Length);

            dataStream.Write(data, 0, (int)file.Length);

            // close the data
            dataStream.Close();
           
            // remove the file 
            CloudStorage.DeleteFileSystemEntry(newFile);
        }

        [Test()]
        public void UploadBinaryComfortTest()
        {
            // set the source file
            String filePath;
            if ( Environment.OSVersion.Platform == PlatformID.Win32NT)
				filePath = "C:\\windows\\notepad.exe";
			else
				filePath = "/usr/bin/which";


            // get the public folder
            ICloudDirectoryEntry publicFolder = CloudStorage.GetFolder("/Public");
            Assert.IsNotNull(publicFolder, "Couldn't get public folder");

            // upload the file 
            Assert.IsTrue(CloudStorage.UploadFile(filePath, publicFolder));

            // upload the file 
            Assert.IsTrue(CloudStorage.UploadFile(filePath, "/Public"));
        }

        [Test()]
        public void DownloadGifTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // find the right file
            ICloudFileSystemEntry readFile = root.GetChild("hhw.gif");
            Assert.IsNotNull(readFile);
            Assert.IsNotInstanceOfType(typeof(ICloudDirectoryEntry), readFile);
            
            // get the stream
            Stream data = readFile.GetContentStream(FileAccess.Read);
            Assert.IsNotNull(data);

            // read the data
            Byte[] dataBuffer = new Byte[data.Length];
            int iReadBytes = data.Read(dataBuffer, 0, (int)data.Length);
            Assert.AreNotEqual(iReadBytes, 0);
            Assert.AreEqual(iReadBytes, data.Length);

            // close the stream
            data.Close();
        }

        [Test()]
        public void DownloadGifCCTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // find the right file
            ICloudFileSystemEntry readFile = root.GetChild("hhw,,.gif");
            Assert.IsNotNull(readFile);
            Assert.IsNotInstanceOfType(typeof(ICloudDirectoryEntry), readFile);

            // get the stream
            Stream data = readFile.GetContentStream(FileAccess.Read);
            Assert.IsNotNull(data);

            // read the data
            Byte[] dataBuffer = new Byte[data.Length];
            int iReadBytes = data.Read(dataBuffer, 0, (int)data.Length);
            Assert.AreNotEqual(iReadBytes, 0);
            Assert.AreEqual(iReadBytes, data.Length);

            // close the stream
            data.Close();
        }

        [Test()]
        public void DownloadGifComfortTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // download the file
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)					
            	CloudStorage.DownloadFile(root, "hhw.gif", "%temp%");
			else
				CloudStorage.DownloadFile(root, "hhw.gif", "/tmp");

            // download the file with second methid
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)					
            	CloudStorage.DownloadFile("/hhw.gif", "%temp%");
			else
				CloudStorage.DownloadFile("/hhw.gif", "/tmp");
        }

        [Test()]
        public void DownloadNotExistingFile()
        {
            // open the root folder object
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            Boolean bGotException = false;

            try
            {
                // download the file
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    CloudStorage.DownloadFile(root, "/MissingFile.missing", "%temp%");
                else
                    CloudStorage.DownloadFile(root, "/MissingFile.missing", "/tmp");
            }
            catch (SharpBoxException e)
            {
                if (e.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
                    bGotException = true;
            }

            Assert.IsTrue(bGotException);
        }

        [Test()]
        public void SerializableTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            { // write stuff
                // create the upload file
                ICloudFileSystemEntry newFile = CloudStorage.CreateFile(null, SerializableFileName);
                Assert.IsNotNull(newFile);

                // content stream
                Stream dataStream = newFile.GetContentStream(FileAccess.Write);
                Assert.IsNotNull(dataStream);

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
                bformatter.Serialize(dataStream, sample);

                // close the stream
                dataStream.Close();
            }

            {   // read stuff

                // read the file
                ICloudFileSystemEntry readFile = root.GetChild(SerializableFileName);
                Assert.IsNotNull(readFile);

                // content stream
                Stream dataStream = readFile.GetContentStream(FileAccess.Read);
                Assert.IsNotNull(dataStream);

                // deserialize with formatter
                BinaryFormatter bformatter = new BinaryFormatter();
                SerializableClassForTest obj = bformatter.Deserialize(dataStream) as SerializableClassForTest;
                Assert.IsNotNull(obj);

                Assert.AreEqual(obj.strValue, SerializableTestStrValue);
                Assert.AreEqual(obj.iValue, SerializableTestiValue);
                Assert.AreEqual(obj.fValue, SerializableTestfValue);

                Assert.IsNotNull(obj.refValue);                

                Assert.AreEqual(obj.refValue.strValue, SerializableTestStrValue);
                Assert.AreEqual(obj.refValue.iValue, SerializableTestiValue);
                Assert.AreEqual(obj.refValue.fValue, SerializableTestfValue);

                // close the stream
                dataStream.Close();
				
				// remove the file 
            	CloudStorage.DeleteFileSystemEntry(readFile);
            }			   			
        }

        [Test()]
        public void UploadPPTXFile()
        {
            String fileName = "xxx.pptx";

            // open the public folder
            ICloudDirectoryEntry publicFolder = CloudStorage.GetFolder("/Public");

            // upload file
            CloudStorage.UploadFile("C:\\" + fileName, publicFolder);
 
            // download file
            String targetDir = Environment.GetEnvironmentVariable("temp");
            CloudStorage.DownloadFile(publicFolder, fileName, targetDir);

            // compare files
            String left = "C:\\" + fileName;
            String right = Path.Combine(targetDir, fileName);

            String md5Left = GenerateMD5Hash(left);
            String md5Right = GenerateMD5Hash(right);

            // remove files
            File.Delete(right);

            // check the hashes
            Assert.AreEqual(md5Left, md5Right);            
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

    }
}
