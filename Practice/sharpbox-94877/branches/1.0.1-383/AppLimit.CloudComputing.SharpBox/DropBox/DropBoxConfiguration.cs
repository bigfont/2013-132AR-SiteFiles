using System;

using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.DropBox
{
    /// <summary>
    /// DropBoxConfiguration conains the default access information for the DropBox
    /// storage and synchronization services. This class implements from the 
    /// ICloudStorageConfiguration interface and can be used with an instance of CloudStorage    
    /// </summary>
    public class DropBoxConfiguration : ICloudStorageConfiguration
    {        
        private String Server                { get { return "api.getdropbox.com"; } }
        private String ContentServer         { get { return "api-content.getdropbox.com"; } }
        private Int32 Port                   { get { return 80; } }

        /// <summary>
        /// Constructor of a dropbox configuration
        /// </summary>
        public DropBoxConfiguration()
        {            
        }

        /// <summary>
        /// This method creates and returns the url which has to be used to get a request token 
        /// during the OAuth based authorization process
        /// </summary>
        /// <returns>Url the receive a request token by oAuth</returns>
        public Uri GetRequestTokenUrl()
        {
            return new Uri("http://" + Server + "/0/oauth/request_token");
        }

        /// <summary>
        /// This method creates and returns the url which has to be used to get a access token 
        /// during the OAuth based authorization process
        /// </summary>
        /// <returns>Url the receive a access token by oAuth</returns>
        public Uri GetAccessTokenUrl()
        {
            return new Uri("http://" + Server + "/0/oauth/access_token");
        }

        /// <summary>
        /// This method creates and returns the url which can be used in the browser
        /// to authorize the end user
        /// </summary>
        /// <returns>Authorization-URL for the end user</returns>
        public Uri GetAuthorizationTokenUrl()
        {
            return new Uri("http://" + Server + "/0/oauth/authorize");
        }
        
        /// <summary>
        /// This method generates an instance of the default dropbox configuration
        /// </summary>
        /// <returns>Instance of the DropBoxConfiguration-Class with default settings</returns>
        static public DropBoxConfiguration GetStandardConfiguration()
        {
            return new DropBoxConfiguration();
        }
        
        /// <summary>
        /// Indicates if the authorization process should be displayed
        /// </summary>
        public bool HasToShowAuthorizationProcess
        {
            get;
            set;
        }
    }
}
