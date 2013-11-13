using System;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
	public class AccountDatabaseEx
	{       
        private static string _accountDBPath = "..\\..\\..\\..\\..\\..\\Configurations\\";
        private static string _accountDBFile = "accounts.xml";
		
		public static string appTagDropBox = "dropboxapp2";
		public static string appUserDropBox = "dropbox2";        
        public static string appUserDWebDav = "webdav2";
        public static string appUserDWebDavExternal = "webdav1";
        public static string appUserDWebDavInternal = "webdav2";
        public static string appUserDWebDavBoxNet = "boxnet";
        public static string appUserDWebDavStoreGate = "storegate2";
        public static string appUserDWebDavCloudMe = "cloudme";
        public static string appUserDFTP = "ftp01";
        public static string appUserHiDrive = "hidrive";
		
		public static AccountDatabase GetAccount(String Tag)
		{
            String accountDBPath = AdjustAccountDbPath(_accountDBFile);
							
			return AccountDatabase.CreateByDatabase(accountDBPath, Tag);	
		}
       
        public static ICloudStorageAccessToken GetTokenFile(String fileName)
        {
            String accountDBPath = AdjustAccountDbPath(fileName);

            using(FileStream fs = File.OpenRead( accountDBPath ))
            {
                CloudStorage cs = new CloudStorage();
                return cs.DeserializeSecurityToken(fs);
            }
        }

        private static String AdjustAccountDbPath(String filename)
        {
            // adjust the account path
            String accountDBPath = Path.Combine(_accountDBPath, filename);

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                accountDBPath = _accountDBPath.Replace("\\", "/");
            }
            return accountDBPath;
        }
	}
}

