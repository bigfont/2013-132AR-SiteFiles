using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

using AppLimit.CloudComputing.OAuth.Token;

namespace AppLimit.CloudComputing.OAuth.Impl
{
    internal class OAuthStreamParser
    {
        private const int _bufferSize = 1024;

        static public OAuthToken ParseTokenInformation(Stream data)
        {
            Dictionary<String, String> parameters = ParseParameterResult(data);            
            return new OAuthToken(parameters["oauth_token"], parameters["oauth_token_secret"]);
        }



        static private Dictionary<String, String> ParseParameterResult(Stream data)
        {        
            String result = GetResultString(data);

            if (result.Length > 0)
            {
                Dictionary<String, String> parsedParams = new Dictionary<string, string>();

                // 1. split at "&"
                String[] parameters = result.Split('&');

                foreach (String paramSet in parameters)
                {
                    String[] param2 = paramSet.Split('=');
                    parsedParams.Add(param2[0], param2[1]);
                }

                return parsedParams;
            }

            return null;
        }


        static private String GetResultString(Stream data)
        {            
            StreamReader reader = new StreamReader(data);
            return reader.ReadToEnd();         
        }

    }
}
