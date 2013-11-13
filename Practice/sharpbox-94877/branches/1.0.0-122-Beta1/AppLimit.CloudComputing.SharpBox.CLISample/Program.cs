using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.CLISample
{
    class Program
    {        
        static void Main(string[] args)
        {
            // read the credentials
            AccountDatabase dropBoxSecrets = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropboxapp");
            AccountDatabase userAccount = AccountDatabase.CreateByDatabase("..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml", "dropbox");

            // build dropbox credentials
            DropBox.DropBoxCredentials creds = new AppLimit.CloudComputing.SharpBox.DropBox.DropBoxCredentials();
            creds.ConsumerKey = dropBoxSecrets.User;
            creds.ComsumerSecret = dropBoxSecrets.Password;
            creds.UserName = userAccount.User;
            creds.Password = userAccount.Password;

            // build up the storage
            CloudStorage storage = new CloudStorage();

            // open the dropbox
            if (!storage.Open(DropBox.DropBoxConfiguration.GetStandardConfiguration(), creds))
                return;
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
            credentials.ComsumerSecret = "TheApplicationSecret";

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
            credentials.ComsumerSecret = "TheApplicationSecret";

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
            credentials.ComsumerSecret = "TheApplicationSecret";

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
            credentials.ComsumerSecret = "TheApplicationSecret";

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
    }
}
