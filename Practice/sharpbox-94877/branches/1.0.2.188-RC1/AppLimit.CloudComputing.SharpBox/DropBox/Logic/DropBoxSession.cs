using System;
using System.Collections.Generic;
using System.IO;

using AppLimit.CloudComputing.OAuth;
using AppLimit.CloudComputing.OAuth.Context;
using AppLimit.CloudComputing.OAuth.Impl;
using AppLimit.CloudComputing.OAuth.Token;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxSession
    {
        public OAuthToken AccessToken { get; set; }
        public OAuthConsumerContext Context { get; set; }

		public Boolean SandBoxMode { get; set; }
		
        public DropBoxSession()
        {
			SandBoxMode = false;
		}

        public DropBoxSession(OAuthToken token, OAuthConsumerContext context)
			: this()
        {
            AccessToken = token;
            Context = context;
        }

        public Stream RequestRessourceStreamByUrl(String url)
        {            
            return OAuthService.GetProtectedRessource(url, Context, AccessToken, null);
        }

        public String RequestRessourceByUrl(String url)
        {
            Stream s = RequestRessourceStreamByUrl(url);

            if (s == null)
                return "";

            var reader = new StreamReader(s);
            return reader.ReadToEnd();            
        }

        public String RequestRessourceByUrl(String url, Dictionary<String, String> parameters)
        {            
            Stream s = OAuthService.GetProtectedRessource(url, Context, AccessToken, parameters);

            if (s == null)
                return "";

            var reader = new StreamReader(s);
            return reader.ReadToEnd();            
        }        
        
        public String PostRessourcByUrl(String url, Dictionary<String, String> parameters, Dictionary<String, Byte[]> mulitpartFormData)
        {            
            Stream s = OAuthService.PostDataIntoProtectedRessource(url, Context, AccessToken, parameters, mulitpartFormData);

            if (s == null)
                return "";

            var reader = new StreamReader(s);
            return reader.ReadToEnd();
        }
    }
}
