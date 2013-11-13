using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

using AppLimit.CloudComputing.OAuth.Context;
using AppLimit.CloudComputing.OAuth.Impl;
using AppLimit.CloudComputing.OAuth.Token;

namespace AppLimit.CloudComputing.OAuth
{
    public class OAuthService
    {
        static public OAuthToken GetRequestToken(OAuthServiceContext svcContext, OAuthConsumerContext conContext)
        {
            // generate the url
            String requestTokenUrl = OAuthUrlGenerator.GenerateRequestTokenUrl(svcContext.RequestTokenUrl, conContext);

            // get the token
            return PerformTokenRequest(requestTokenUrl);                       
        }        

        static public String GetAuthorizationUrl(OAuthServiceContext svcContext, OAuthToken requestToken)
        {
            return OAuthUrlGenerator.GenerateAuthorizationUrl(svcContext.AuthorizationUrl, svcContext.CallbackUrl, requestToken);
        }

        static public OAuthToken GetAccessToken(OAuthServiceContext svcContext, OAuthConsumerContext conContext, OAuthToken requestToken)
        {
            String url = OAuthUrlGenerator.GenerateAccessTokenUrl(svcContext.AccessTokenUrl, conContext, requestToken);

            return PerformTokenRequest(url);                       
        }

        static public Stream GetProtectedRessource(String ressourcUrl, OAuthConsumerContext conContext, OAuthToken accessToken, Dictionary<String, String> parameter)
        {
            // method
            const string webMethod = "GET";

            // build url
            String url = OAuthUrlGenerator.GenerateSignedUrl(ressourcUrl, webMethod, conContext, accessToken, parameter);

            // form a webrequest
            return PerformWebRequest(url, webMethod, null);
        }

        static public Stream PostDataIntoProtectedRessource(String ressourceUrl, OAuthConsumerContext conContext, OAuthToken accessToken, Dictionary<String, String> parameter, Dictionary<String, Byte[]> multiFormDataStream )
        {
            // method
            const string webMethod = "POST";

            // build url
            String url = OAuthUrlGenerator.GenerateSignedUrl(ressourceUrl, webMethod, conContext, accessToken, parameter);

            // form a webrequest
            return PerformWebRequest(url, webMethod, multiFormDataStream);
        }

        private static Stream PerformWebRequest(String url, String method, Dictionary<String, Byte[]> multiFormDataStream)
        {
            // form a webrequest
            var request = HttpWebRequest.Create(url);
            request.Method = method;

            if (method != "GET" && multiFormDataStream != null)
            {
                var fds = new OAuthMultipartFormDataSupport();                
                request.ContentType = fds.GetMultipartFormContentType();

                using (Stream requestStream = request.GetRequestStream())
                {
                    fds.WriteMultipartFormData(requestStream, multiFormDataStream);
                }
            }
            else
                request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                // check the rsult code
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.GetResponseStream();                    
                }
                else
                    return null;
            }
            catch (WebException)
            {                
                return null;
            }
        }        

        private static OAuthToken PerformTokenRequest(String requestTokenUrl)
        {
        	Stream s = PerformWebRequest(requestTokenUrl, "GET", null);
        	return s != null ? OAuthStreamParser.ParseTokenInformation(s) : null;
        }
    }
}
