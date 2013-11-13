using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web;
using System.Text;

using AppLimit.CloudComputing.SharpBox.OAuth;
using AppLimit.CloudComputing.SharpBox.OAuth.Context;
using AppLimit.CloudComputing.SharpBox.OAuth.Impl;
using AppLimit.CloudComputing.SharpBox.OAuth.Token;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxSession
    {
        public OAuthToken AccessToken { get; set; }
        public OAuthConsumerContext Context { get; set; }

		public Boolean bSandBoxMode;
		
        public DropBoxSession()
        {
			bSandBoxMode = true;
		}

        public DropBoxSession(OAuthToken token, OAuthConsumerContext context)
			: this()
        {
            this.AccessToken = token;
            this.Context = context;
        }

        public Stream RequestRessourceStreamByUrl(String url)
        {            
            return OAuthService.GetProtectedRessource(url, this.Context, this.AccessToken, null);
        }

        public String RequestRessourceByUrl(String url)
        {
            Stream s = RequestRessourceStreamByUrl(url);

            if (s == null)
                return "";

            StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();            
        }

        public String RequestRessourceByUrl(String url, Dictionary<String, String> parameters)
        {            
            Stream s = OAuthService.GetProtectedRessource(url, this.Context, this.AccessToken, parameters);

            if (s == null)
                return "";

            StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();            
        }        
        
        public String PostRessourcByUrl(String url, Dictionary<String, String> parameters, Dictionary<String, Byte[]> mulitpartFormData)
        {            
            Stream s = OAuthService.PostDataIntoProtectedRessource(url, this.Context, this.AccessToken, parameters, mulitpartFormData);

            if (s == null)
                return "";

            StreamReader reader = new StreamReader(s);
            return reader.ReadToEnd();
        }
    }
}
