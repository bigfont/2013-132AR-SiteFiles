using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using AppLimit.CloudComputing.SharpBox.NUnit;

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
            StorageProvider.DropBox.DropBoxCredentials creds = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();
            creds.ConsumerKey = DropBoxSecret.User;
            creds.ConsumerSecret = DropBoxSecret.Password;
            creds.UserName = UserAccount.User;
            creds.Password = UserAccount.Password;

            // build up the storage
            CloudStorage storage = new CloudStorage();

            // open the dropbox
			Console.WriteLine("Opening Storage");
            storage.Open(StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration(), creds);                
           
            // get public folder
            ICloudDirectoryEntry fld = storage.GetFolder("/Public");
            // ICloudFileSystemEntry f = fld.GetChild("te'st.txt");

            storage.DownloadFile(fld, "te'st.txt", "%appdata%");



            // build our business object
			Console.WriteLine("Writing sample object");
            SampleClass sample = new SampleClass();
            sample.value1 = "Hello";
            sample.value2 = "World";
            sample.iv1 = 1;
            sample.iv2 = 2;
            
            // get the cloud file 
            ICloudFileSystemEntry file = storage.CreateFile(null, "ser.dat");
            
            // build the serialization formatter
            BinaryFormatter bformatter = new BinaryFormatter();
            
            // serialize
            file.GetDataTransferAccessor().Serialize(bformatter, sample);            
            
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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";
            
            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);            

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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);            

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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);            

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

                // build a caching stream
                MemoryStream stream = new MemoryStream();

                // download the data
                file.GetDataTransferAccessor().Transfer(stream, nTransferDirection.nDownload, null, null);
                
                // go to start
                stream.Position = 0;

                // build a stream read
                StreamReader reader = new StreamReader(stream);
                Console.WriteLine("Info: {0}", reader.ReadToEnd());                

                // close the streamreader and the stream
                reader.Close();                
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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);            

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

                // build the data stream
                Stream data = new MemoryStream();

                // build a stream read
                StreamWriter writer = new StreamWriter(data);
                writer.WriteLine("Hello World");

                // reset stream
                data.Position = 0;

                // upload data
                file.GetDataTransferAccessor().Transfer(data, nTransferDirection.nUpload, null, null);
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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);            

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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);            

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
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);           

            // create a very long folder paht
            storage.CreateFolder("/Public/My/Very/Long/Folder/Path/Which/Will/Generated/In/Just/One/Call");
            
            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }
		
		static void DeleteFileSystemEntrySimple()
        {
            // instanciate a new credentials object, e.g. for dropbox
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);           

            // delete a file
            storage.DeleteFileSystemEntry("/Public/My/File.sample");
			
			// delete a directory
            storage.DeleteFileSystemEntry("/Public/My/Directory");
            
            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }
		
		static void MoveFileSystemEntrySimple()
        {
            // instanciate a new credentials object, e.g. for dropbox
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);           

            // move a file to /Public folder
            storage.MoveFileSystemEntry("/Public/My/File.sample", "/Public");
			
			// move a directory to root
            storage.DeleteFileSystemEntry("/Public/My/Directory");
            
            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }
		
		static void RenameFileSystemEntrySimple()
        {
            // instanciate a new credentials object, e.g. for dropbox
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);           

            // move a file to /Public folder
            storage.RenameFileSystemEntry("/Public/My/File.sample", "NewFile.sample");
			
			// move a directory to root
            storage.RenameFileSystemEntry("/Public/My/Directory", "NewDirectory");
            
            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }
		
		static void CreateFile()
        {
            // instanciate a new credentials object, e.g. for dropbox
            StorageProvider.DropBox.DropBoxCredentials credentials = new AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.DropBoxCredentials();

            // attach the application information 
            credentials.ConsumerKey = "TheApplicationKey";
            credentials.ConsumerSecret = "TheApplicationSecret";

            // add the account information
            credentials.UserName = "myaccount@dropbox.com";
            credentials.Password = "mypassword";

            // instanciate a cloud storage configuration, e.g. for dropbox
            StorageProvider.DropBox.DropBoxConfiguration configuration = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // instanciate the cloudstorage manager
            CloudStorage storage = new CloudStorage();

            // open the connection to the storage
            storage.Open(configuration, credentials);           

            // move a file to /Public folder
            storage.CreateFile("/Public/My/File.sample");
            
            // close the cloud storage connection
            if (storage.IsOpened)
            {
                storage.Close();
            }
        }

        static void LoginViaAuthURLForWebApps()
        {
            // 0. load the comsumer key
            AccountDatabase DropBoxSecret = null;			
			
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix )
			{
				DropBoxSecret = AccountDatabase.CreateByDatabase("../../../../../../Configurations/accounts.xml", "dropboxapp2");           		
			}
			else
			{
				DropBoxSecret = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp2");           		
			}

            String ConsumerKey = DropBoxSecret.User;
            String ComsumerSecret = DropBoxSecret.Password;

            // 0. load the config
            DropBoxConfiguration config = StorageProvider.DropBox.DropBoxConfiguration.GetStandardConfiguration();

            // 1. get the request token from dropbox
            DropBoxRequestToken requestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken(config, ConsumerKey, ComsumerSecret);

            // 2. build the authorization url based on request token                        
            String url = DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl(config, requestToken);

            // 3. Redirect the user to the website of dropbox
            // ---> DO IT <--
            // ---> if not, you will get an unauthorized exception <--

            // 4. Exchange the request token into access token
            ICloudStorageAccessToken accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config, ConsumerKey, ComsumerSecret, requestToken);

            // 5. Opent the storage with the generated access token            
            CloudStorage storageNew = new CloudStorage();
            storageNew.Open(config, accessToken);

            // 6. Try to do something
            ICloudDirectoryEntry r = storageNew.GetRoot();
        }

        static void EnumDirectoryEntry(ICloudDirectoryEntry entry)
        {
            foreach (ICloudFileSystemEntry fs in entry)
            {
                Console.WriteLine(fs.Name);

                if (fs is ICloudDirectoryEntry)
                    EnumDirectoryEntry(fs as ICloudDirectoryEntry);
            }            
        }        
    }
}
