using System;

using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.DropBox
{   
    /// <summary>
    /// This class contains the needed access credentials for a specific dropbox
    /// application sandbox and a specific end user
    /// </summary>
    public class DropBoxCredentials : ICloudeStorageCredentials
    {
        /// <summary>
        /// ConsumerKey of the DropBox application, provided by the 
        /// DropBox-Developer-Portal
        /// </summary>
        public String ConsumerKey;

        /// <summary>
        /// ConsumerSecret of the DropBox application, provided by the 
        /// DropBox-Developer-Portal
        /// </summary>
        public String ComsumerSecret;

        /// <summary>
        /// Useraccount of the end user with access to DropBox and the 
        /// defined application
        /// </summary>
        public String UserName;

        /// <summary>
        /// Password of the end user with access to DropBox and the 
        /// defined application
        /// </summary>
        public String Password;
    }
}
