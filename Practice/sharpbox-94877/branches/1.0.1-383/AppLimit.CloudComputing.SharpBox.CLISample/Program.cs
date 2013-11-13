using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.CLISample
{
    class Program
    {        
        static void Main(string[] args)
        {
            // read the credentials   			
			Console.WriteLine("Reading application and user information");
            AccountDatabase DropBoxSecret = null;
			AccountDatabase UserAccount = null;
			
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix )
			{
				DropBoxSecret = AccountDatabase.CreateByDatabase("../../../../../../Configurations/accounts.xml", "dropboxapp2");
           		UserAccount = AccountDatabase.CreateByDatabase("../../../../../../Configurations/accounts.xml", "dropbox2");				
			}
			else
			{
				DropBoxSecret = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp2");
           		UserAccount = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropbox2");
			}							
			
            // build dropbox credentials
			Console.WriteLine("Building credential objects");
            DropBox.DropBoxCredentials creds = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            creds.ConsumerKey = DropBoxSecret.User;
            creds.ConsumerSecret = DropBoxSecret.Password;
            creds.UserName = UserAccount.User;
            creds.Password = UserAccount.Password;

            // build up the storage
            CloudStorage storage = new CloudStorage();

            // open the dropbox
			Console.WriteLine("Opening Storage");
            if (!storage.Open(DropBox.DropBoxConfiguration.GetStandardConfiguration(), creds))
                return;


            // build our business object
			Console.WriteLine("Writing sample object");
            SampleClass sample = new SampleClass();
            sample.value1 = "Hello";
            sample.value2 = "World";
            sample.iv1 = 1;
            sample.iv2 = 2;
            
            // open the file stream
            ICloudFileSystemEntry file = storage.CreateFile(null, "ser.dat");
            Stream stream = file.GetContentStream(FileAccess.Write);

            // serialize with formatter
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, sample);

            // close the stream
			Console.WriteLine("Flushing caches");
            stream.Close();

            // close the dropbox
			Console.WriteLine("Closing storage");
            if (storage.IsOpened)
                storage.Close();
        }

        static private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();

            return builder.ToString();
        }

        static void CreateDirectory()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";
            
            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

            // create the folder in the root directory
            ICloudDirectoryEntry newFolder = storage.CreateFolder("MyFirstFolder", null);
            if (newFolder == null)
            {
                Console.WriteLine("Couldn't create MyFirstFolder");
            }

            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }

        static void EnumerateFilesAndFolder()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

            // get the root entry of the cloud storage 
            ICloudDirectoryEntry root = storage.GetRoot();
            if (root == null)
            {
                Console.WriteLine("No root object found");
            }
            else
            {
                foreach (ICloudFileSystemEntry fsentry in root)
                {
                    if (fsentry is ICloudDirectoryEntry)
                    {
                        Console.WriteLine("Found Directory: {0}", fsentry.Name);
                    }
                    else
                    {
                        Console.WriteLine("Found File: {0}", fsentry.Name);
                    }
                }
            }

            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }

        static void DownloadFile()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

               // get the root entry of the cloud storage 
            ICloudDirectoryEntry root = storage.GetRoot();
            if (root == null)
            {
                Console.WriteLine("No root object found");
            }
            else
            {
                // get the file entry
                ICloudFileSystemEntry file = root.GetChild("xxx.txt");

                // download the data
                Stream data = file.GetContentStream(FileAccess.Read);

                // build a stream read
                StreamReader reader = new StreamReader(data);
                Console.WriteLine("Info: {0}", reader.ReadToEnd());                

                // close the streamreader
                reader.Close();

                // close the stream
                data.Close();
            }

            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }

        static void UploadFile()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

               // get the root entry of the cloud storage 
            ICloudDirectoryEntry root = storage.GetRoot();
            if (root == null)
            {
                Console.WriteLine("No root object found");
            }
            else
            {
                // create the file
                ICloudFileSystemEntry file = storage.CreateFile(root, "up.load");

                // upload the data
                Stream data = file.GetContentStream(FileAccess.Write);

                // build a stream read
                StreamWriter writer = new StreamWriter(data);
                writer.WriteLine("Hello World");

                // close the streamreader
                writer.Close();

                // close the stream
                data.Close();
            }

            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }


        static void PathBasedAccess()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

            // get the folder entry path bases
            ICloudDirectoryEntry publicFolder = storage.GetFolder("/Public");

            //
            // add your code her 
            //

            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }

        static void DownloadFileSimple()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

            // download your file from the public folder
            storage.DownloadFile("/Public/test,dat", "%temp%");

            // upload your file to the public folder
            storage.UploadFile("%temp%\test.dat", "/Public");

            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }

        static void CreateFolderEasy()
        {
            // instanciate a new credentials object, e.g. for dropbox
            DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            DropBox.DropBoxConfiguration configuration = DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            if (!storage.Open(configuration, credentials))
            {
                Console.WriteLine("Connection failed");
                return;
            }

            // create a very long folder paht
            storage.CreateFolder("/Public/My/Very/Long/Folder/Path/Which/Will/Generated/In/Just/One/Call");
            
            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }
    }
}
