using System;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
	public class AccountDatabaseEx
	{
		private static string _accountDBPath = "..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml";
		
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
			// adjust the account path
			String accountDBPath = _accountDBPath;
						
			if ( Environment.OSVersion.Platform != PlatformID.Win32NT )
			{
				accountDBPath = _accountDBPath.Replace("\\", "/");	
			}
			
				
			return AccountDatabase.CreateByDatabase(accountDBPath, Tag);	
		}
	}
}

