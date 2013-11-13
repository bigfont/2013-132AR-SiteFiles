using System;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
	public class AccountDatabaseEx
	{
		private static string _accountDBPath = "..\\..\\..\\..\\..\\..\\Configurations\\accounts.xml";
		
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

