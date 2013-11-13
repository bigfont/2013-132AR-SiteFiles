using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace AppLimit.CloudComputing.OAuth
{
    public delegate void WebRequestExecuted(Object sender, WebRequest request, TimeSpan timeNeeded, HttpStatusCode statusCode, Stream resultStream);
    public delegate void WebRequestExecuting(Object sender, WebRequest request);

    /// <summary>
    /// This class allows to be informed about all oAuth requests in the 
    /// library. Do not add event receiver during runtime, only during start
    /// up of your application
    /// </summary>
    public sealed class OAuthServiceManager
    {
        #region Singleton Stuff

        static readonly OAuthServiceManager instance = new OAuthServiceManager();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static OAuthServiceManager()
        {
        }

        OAuthServiceManager()
        {
#if !WINDOWS_PHONE                        
            _webProxySettings = null;
#endif
        }

        public static OAuthServiceManager Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

#if !WINDOWS_PHONE            
        private WebProxy _webProxySettings;
#endif
        public event WebRequestExecuting requestExecutingEvent;
        public event WebRequestExecuted requestExecutedEvent;

        public void NotifyWebRequestExecuting(WebRequest request)
        {
            if (requestExecutingEvent != null)
                requestExecutingEvent(this, request);
        }

        public void NotifyWebRequestExecuted(WebRequest request, TimeSpan timeNeeded, HttpStatusCode statusCode, Stream resultStream)
        {
            if (requestExecutedEvent != null)
                requestExecutedEvent(this, request, timeNeeded, statusCode, resultStream);
        }

#if !WINDOWS_PHONE            
        public void SetProxySettings(String proxyHost, int proxyPort, ICredentials credentials)
        {
            if (proxyHost != null)
            {
                _webProxySettings = new WebProxy(proxyHost, proxyPort);
                _webProxySettings.Credentials = credentials;
            }
            else
                _webProxySettings = null;
        }

        internal WebProxy GetProxySettings()
        {
            return _webProxySettings;
        }
#endif

    }


}
