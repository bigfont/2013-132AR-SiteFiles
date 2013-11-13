using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MySharpBox
{
    class Program
    {
        static void Main(string[] args)
        {
            // enter the comsumer key and secret
            ////String ConsumerKey = "sp2p8ei6wvnn62t";
            ////String ComsumerSecret = "jyayox3moai0h9v";

            CloudStorage dropBoxStorage = new CloudStorage();

            var dropBoxConfig =
                CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);

            // declare an access token
            ICloudStorageAccessToken accessToken = null;

            // load a valid security token from file 
            using (FileStream fs = File.Open("SharpDropBox.Token", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                accessToken = dropBoxStorage.DeserializeSecurityToken(fs);
            }

            // open the connection 
            ICloudStorageAccessToken storageToken = dropBoxStorage.Open(dropBoxConfig, accessToken);

            DropBoxAccountInfo info = DropBoxStorageProviderTools.GetAccountInformation(storageToken);

            /*

            ICloudDirectoryEntry root = dropBoxStorage.GetRoot();
            
            foreach (var dunno in dropBoxStorage.GetRoot())
            {
                if (dunno is ICloudFileSystemEntry && !(dunno is ICloudDirectoryEntry))
                {
                   Console.WriteLine(dunno.Name);
                   dropBoxStorage.DownloadFile(root, dunno.Name,
                    Environment.ExpandEnvironmentVariables("%temp%"));
                }
            }

            // upload a testfile from temp directory into public folder of DropBox
            string myFileName = "test.dat";
            String srcFile = Environment.ExpandEnvironmentVariables("%temp%\\" + myFileName);
            dropBoxStorage.UploadFile(srcFile, root);
            
             
             */

            Console.ReadLine();

            // close the connection 
            dropBoxStorage.Close();
        }
    }
}
