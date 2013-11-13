using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

using AppLimit.CloudComputing.OAuth;
using AppLimit.CloudComputing.OAuth.Context;
using AppLimit.CloudComputing.OAuth.Impl;
using AppLimit.CloudComputing.OAuth.Token;

#if SILVERLIGHT
using  AppLimit.CloudComputing.oAuth.WP7.SilverLightHelper;
#endif

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

        static public Stream GetProtectedResource(String resourceUrl, OAuthConsumerContext conContext, OAuthToken accessToken, Dictionary<String, String> parameter)
        {
            // method
            const string webMethod = "GET";

            String url = GetProtectedResourceUrl(resourceUrl, conContext, accessToken, parameter, webMethod);

            // form a webrequest
            return PerformWebRequest(url, webMethod, null);
        }

        public static String GetProtectedResourceUrl(String resourceUrl, OAuthConsumerContext conContext, OAuthToken accessToken, Dictionary<String, String> parameter, string webMethod)
        {
            // build url
            String url = OAuthUrlGenerator.GenerateSignedUrl(resourceUrl, webMethod, conContext, accessToken, parameter);

            return url;
        }

        static public Stream PostDataIntoProtectedResource(String resourceUrl, OAuthConsumerContext conContext, OAuthToken accessToken, Dictionary<String, String> parameter, Dictionary<String, Byte[]> multiFormDataStream)
        {
            // method
            const string webMethod = "POST";

            // build url
            String url = OAuthUrlGenerator.GenerateSignedUrl(resourceUrl, webMethod, conContext, accessToken, parameter);

            // form a webrequest
            return PerformWebRequest(url, webMethod, multiFormDataStream);
        }

        private static Stream PerformWebRequest(String url, String method, Dictionary<String, Byte[]> multiFormDataStream)
        {            
            // init the stop watch
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // return value
            Stream retStream = null;
            HttpStatusCode retStatus = HttpStatusCode.OK;

            // form a webrequest
            var request = HttpWebRequest.Create(url);
            request.Method = method;

#if !WINDOWS_PHONE            
            // set the proxy class if neede
            if (OAuthServiceManager.Instance.GetProxySettings() != null)
                request.Proxy = OAuthServiceManager.Instance.GetProxySettings();
#endif
            
            if (method != "GET")
            {
                if (multiFormDataStream != null)
                {
                    var fds = new OAuthMultipartFormDataSupport();
                    request.ContentType = fds.GetMultipartFormContentType();

                    using (Stream requestStream = OAuthStreamHelper.GetRequestStream(request))
                    {
                        fds.WriteMultipartFormData(requestStream, multiFormDataStream);
                    }
                }
                else
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
            }

            try
            {
                // notify about the execution 
                // OAuthServiceManager.Instance.NotifyWebRequestExecuting(request);
                var response = OAuthStreamHelper.GetResponse( request ) as HttpWebResponse;

                // set the result code
                retStatus = response.StatusCode;

                // get the response stream
                retStream = response.GetResponseStream();
                
            }
            catch (WebException e)
            {                                
                // update the error code
                HttpWebResponse resp = e.Response as HttpWebResponse;
                retStatus = resp.StatusCode;
                
                // reset the return stream
                retStream = resp.GetResponseStream();
            }
            finally
            {
                // stop the watch
                watch.Stop();
                
                // notify
                OAuthServiceManager.Instance.NotifyWebRequestExecuted(request, watch.Elapsed, retStatus, retStream);
            }

            // return the result
            return (retStatus == HttpStatusCode.OK) ? retStream : null;
        }        

        private static OAuthToken PerformTokenRequest(String requestTokenUrl)
        {
        	Stream s = PerformWebRequest(requestTokenUrl, "GET", null);
        	return s != null ? OAuthStreamParser.ParseTokenInformation(s) : null;
        }
    }
}
