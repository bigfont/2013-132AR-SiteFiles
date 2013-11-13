using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

#if WINDOWS_PHONE
using System.Windows.Threading;
using System.Windows.Controls;
#endif

namespace AppLimit.CloudComputing.OAuth
{
    public class OAuthStreamHelper
    {        
        private static Stream GetRequestStreamInternal(WebRequest request)
        {
            // build the envent
            ManualResetEvent revent = new ManualResetEvent(false);

            // start the async call
            IAsyncResult result = request.BeginGetRequestStream(CallStreamCallback, revent);

            // wait for event
            revent.WaitOne();
            
            // return data stream
            return request.EndGetRequestStream(result);
        }
        
        private static WebResponse GetResponseInternal(WebRequest request)
        {            
            // build the envent
            ManualResetEvent revent = new ManualResetEvent(false);

            // start the async call
            IAsyncResult result = request.BeginGetResponse(CallStreamCallback, revent);
            
            // wait for event
            revent.WaitOne();

            // get the response
            WebResponse response = request.EndGetResponse(result);
            
            // go ahead
            return response;            
        }

        private static void CallStreamCallback(IAsyncResult asynchronousResult)
        {
            ManualResetEvent revent = asynchronousResult.AsyncState as ManualResetEvent;
            revent.Set();
        }

        public static Stream GetRequestStream(WebRequest request)
        {
#if WINDOWS_PHONE            
            return OAuthStreamHelper.GetRequestStreamInternal(request);
#else
            return request.GetRequestStream();
#endif
        }

        public static WebResponse GetResponse(WebRequest request)
        {
#if WINDOWS_PHONE            
            return OAuthStreamHelper.GetResponseInternal(request);            
#else
            return request.GetResponse();
#endif
        }
    }
}
