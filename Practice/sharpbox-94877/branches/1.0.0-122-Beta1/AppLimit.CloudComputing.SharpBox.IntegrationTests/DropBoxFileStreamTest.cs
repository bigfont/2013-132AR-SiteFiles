using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.IntegrationTests.UnitTestClasses;

namespace AppLimit.CloudComputing.SharpBox.IntegrationTests
{
    /// <summary>
    /// Summary description for DropBoxFileStreamTest
    /// </summary>
    [TestClass]
    public class DropBoxFileStreamTest : DropBoxTestItem
    {
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


        [TestMethod]
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

        [TestMethod]
        public void UploadNotepadExeTest()
        {
            // create the upload file
            ICloudFileSystemEntry newFile = CloudStorage.CreateFile(null, "notepad.exe");
            Assert.IsNotNull(newFile);

            // content stream
            Stream dataStream = newFile.GetContentStream(FileAccess.Write);

            FileStream file = new FileStream("C:\\windows\\notepad.exe", FileMode.Open, FileAccess.Read);

            Byte[] data = new Byte[file.Length];

            file.Read(data, 0, (int)file.Length);

            dataStream.Write(data, 0, (int)file.Length);
                        
            // close the data
            dataStream.Close();            

            // remove the file 
            CloudStorage.DeleteFileSystemEntry(newFile);
        }

        [TestMethod]
        public void DownloadGifTest()
        {
            // open the root folder object
            ICloudDirectoryEntry root = CloudStorage.GetRoot();
            Assert.IsNotNull(root);

            // find the right file
            ICloudFileSystemEntry readFile = root.GetChild("hhw.gif");
            Assert.IsNotNull(readFile);
            Assert.IsNotInstanceOfType(readFile, typeof(ICloudDirectoryEntry));
            
            // get the stream
            Stream data = readFile.GetContentStream(FileAccess.Read);
            Assert.IsNotNull(data);

            // read the data
            Byte[] dataBuffer = new Byte[data.Length];
            int iReadBytes = data.Read(dataBuffer, 0, (int)data.Length);
            Assert.AreEqual(iReadBytes, data.Length);

            // close the stream
            data.Close();
        }
    }
}
