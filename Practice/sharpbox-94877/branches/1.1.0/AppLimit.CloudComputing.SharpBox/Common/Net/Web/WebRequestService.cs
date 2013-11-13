﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using AppLimit.CloudComputing.SharpBox.Common.IO;

#if WINDOWS_PHONE
using AppLimit.CloudComputing.oAuth.WP7.SilverLightHelper;
#endif

namespace AppLimit.CloudComputing.SharpBox.Common.Net.Web
{
    internal class WebRequestService
    {
        private const string watchTag = "WebRequestServiceRequestWatch";

        #region StopWatch Helper

        private static void StopStopWatch(WebResponse response, Stream ret)
        {
#if !WINDOWS_PHONE
            // 2. stop the watch from tls
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(watchTag);
            Stopwatch watch = Thread.GetData(slot) as Stopwatch;
#else
            Stopwatch watch = null;
#endif

            if (watch != null)
            {
                // stop watch
                watch.Stop();

                // notify
                WebRequestManager.Instance.NotifyWebRequestExecuted(response, watch.Elapsed, ret);
            }
        }

        #endregion

        #region Base Web Methods

        /// <summary>
        /// This method generates a new webrequest and instruments the stopwatch
        /// counter
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="credentials"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual WebRequest CreateWebRequest(String url, ValidRequestMethod method, ICredentials credentials, Object context)
        {
            return CreateWebRequest(url, method, credentials, context, null);
        }

        /// <summary>
        /// The callback 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        public delegate void CreateWebRequestPreparationCallback(WebRequest request, Object context);

        /// <summary>
        /// Internal web request generator
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="credentials"></param>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected WebRequest CreateWebRequest(String url, ValidRequestMethod method, ICredentials credentials, Object context, CreateWebRequestPreparationCallback callback)
        {
            // 1. create and start the watch
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            // 2. build uri
            Uri uriNew = new Uri(url);

            // 2. create request 
            HttpWebRequest request = HttpWebRequest.Create(uriNew) as HttpWebRequest;
            request.Method = method.ToString().ToUpper();

#if !WINDOWS_PHONE
            request.AllowWriteStreamBuffering = false;
            request.Timeout = Timeout.Infinite;
            request.ReadWriteTimeout = Timeout.Infinite;
#endif

            // 3. call back
            if (callback != null)
                callback(request, context);

#if !WINDOWS_PHONE
            // 4. set the proxy class if needed
            if (WebRequestManager.Instance.GetProxySettings() != null)
                request.Proxy = WebRequestManager.Instance.GetProxySettings();

            // 5. set the credentials if needed
            if (credentials != null)
            {
                request.Credentials = credentials;
                request.PreAuthenticate = true;
            }
#endif

            // 6. notify
            WebRequestManager.Instance.NotifyWebRequestPrepared(request);

#if !WINDOWS_PHONE
            // 7. add watch object to tls
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(watchTag);
            Thread.SetData(slot, watch);
#endif

            // 8. return the request
            return request;
        }

        /// <summary>
        /// This method returns a webrequest data stream and should be used in 
        /// a using clause
        /// </summary>
        /// <param name="request"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual WebRequestStream GetRequestStream(WebRequest request, long length)
        {
#if !WINDOWS_PHONE
            // set the conten length
            request.ContentLength = length;
#endif

            // get the network stream
            Stream nwStream = WebRequestStreamHelper.GetRequestStream(request);

            // return the request streaM;
            WebRequestStream ws =  new WebRequestStream(nwStream, request, this);

            // add pre dispose opp
            ws.PushPreDisposeOperation(DisposeWebRequestStreams, request, ws);

            // go ahead
            return ws;
        }

        /// <summary>
        /// This method contains the routine which has to be executed when a request 
        /// stream will be disposed and will be called autoamtically
        /// </summary>
        private void DisposeWebRequestStreams(params object[] arg)
        {
            // get the params
            WebRequest request = arg[0] as WebRequest;
            WebRequestStream stream = arg[1] as WebRequestStream;

            // check if we have a multipart upload
            WebRequestMultipartFormDataSupport md = new WebRequestMultipartFormDataSupport();
            if (md.IsRequestMultiPartUpload(request))
                md.FinalizeNetworkFileDataStream(stream);            
        }

        /// <summary>
        /// This method returns a webresponse or throws an exception. In the case of an 
        /// exception the stop watch is stop here
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual WebResponse GetWebResponse(WebRequest request)
        {            
            // execute the webrequest
            try
            {
#if !WINDOWS_PHONE
                // check the length
                if (request.ContentLength == -1)
                {
                    request.ContentLength = 0;
                    request.ContentType = "";
                }
#endif

                // Notify
                WebRequestManager.Instance.NotifyWebRequestExecuting(request);

                // get the response
                return WebRequestStreamHelper.GetResponse(request) as HttpWebResponse;                
            }
            catch (WebException e)
            {
                // stop the watch
                StopStopWatch(null, null);

                // rethrow the exception
                throw e;                
            }            
        }

        /// <summary>
        /// This method returns an response stream 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public virtual Stream GetResponseStream(WebResponse response)
        {
            Stream responseStream = null;

            try
            {
                // get the network stream
                Stream nwStream = WebRequestStreamHelper.GetResponseStream(response);

                // get the response stream            
                responseStream = new WebResponseStream(nwStream, response, this);
            }
            catch (WebException)
            {
                return null;
            }
            finally
            {
                StopStopWatch(response, responseStream);
            }

            // return the stream
            return responseStream;
        }

        /// <summary>
        /// This method executes alle dispose code for webresponse streams
        /// </summary>
        /// <param name="response"></param>
        /// <param name="stream"></param>
        public virtual void DisposeWebResponseStreams(WebResponse response, WebResponseStream stream)
        {            
        }

        #endregion

        #region Comfort Functions

        /// <summary>
        /// Performs a five webrequest and returns the result as an in memory stream
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>         
        public MemoryStream PerformWebRequest2(WebRequest request, Object context)
        {
            HttpStatusCode code;
            WebException e;
            return PerformWebRequest2(request, context, out code, out e);
        }
    
        /// <summary>
        /// Performs a five webrequest and returns the result as an in memory stream
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public MemoryStream PerformWebRequest2(WebRequest request, Object context, out HttpStatusCode code, out WebException errorInfo)
        {
            return PerformWebRequest2(request, context, null, out code, out errorInfo);
        }

        /// <summary>
        /// Performs a five webrequest and returns the result as an in memory stream
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="content"></param>
        /// <param name="code"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public MemoryStream PerformWebRequest2(WebRequest request, Object context, Stream content, out HttpStatusCode code, out WebException errorInfo)
        {
            try
            {
                // add content if needed
                if (content != null)
                {
                    using (Stream requestStream = GetRequestStream(request, content.Length))
                    {
                        StreamHelper.CopyStreamData(content, requestStream, null, null);
                    }
                }

                // create the memstream
                MemoryStream memStream = new MemoryStream();

                // get the response
                using (WebResponse response = GetWebResponse(request))
                {
                    // set the error code
                    code = (response as HttpWebResponse).StatusCode;

                    // read the data 
                    using (Stream responseStream = GetResponseStream(response))
                    {
                        // copy the data into memory                        
                        StreamHelper.CopyStreamData(responseStream, memStream, null, null);

                        // reset the memory stream
                        memStream.Position = 0;

                        // close the source stream
                        responseStream.Close();
                    }

                    // close the response
                    response.Close();
                }

                // no error
                errorInfo = null;

                // return the byte stream
                return memStream;
            }
            catch (WebException e)
            {
                if (e.Response == null || (e.Response as HttpWebResponse) == null)
                    code = HttpStatusCode.BadRequest;
                else
                    code = ((HttpWebResponse)e.Response).StatusCode;

                errorInfo = e;

                return null;
            }
        }        

        /// <summary>
        /// Forms a webrequest and performs them. The result will generated as memory stream
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="credentials"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public MemoryStream PerformWebRequest2(String url, ValidRequestMethod method, ICredentials credentials, Object context)
        {
            HttpStatusCode code;
            WebException e;
            return PerformWebRequest2(url, method, credentials, context, out code, out e);    
        }

        /// <summary>
        /// Forms a webrequest and performs them. The result will generated as memory stream 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="credentials"></param>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public MemoryStream PerformWebRequest2(String url, ValidRequestMethod method, ICredentials credentials, Object context, out HttpStatusCode code, out WebException errorInfo)
        {
            return PerformWebRequest2(url, method, credentials, null, context, out code, out errorInfo);
        }
        
        /// <summary>
        /// Forms a webrequest and performs them. The result will generated as memory stream 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="credentials"></param>
        /// <param name="content"></param>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
        public MemoryStream PerformWebRequest2(String url, ValidRequestMethod method, ICredentials credentials, Stream content, Object context, out HttpStatusCode code, out WebException errorInfo)        
        {
            // create a web request
            WebRequest request = CreateWebRequest(url, method, credentials, context);
            if (request == null)
                throw new Exception("Could not generate WebRequest for " + method + ":" + url);

            return PerformWebRequest2(request, context, content, out code, out errorInfo);
        }
        
        #endregion

        #region Multi Part Form Data Support

        /// <summary>
        /// This method implements a standard multipart/form-data upload and can be overriden 
        /// e.g. for WebDav upload
        /// </summary>
        /// <param name="url"></param>        
        /// <param name="credentials"></param>
        /// <returns></returns>
        public virtual WebRequest CreateWebRequestMultiPartUpload(String url, ICredentials credentials)
        {
            // 1. build a valid webrequest
            WebRequest request = CreateWebRequest(url, ValidRequestMethod.Post, credentials, null);

            // 2. set the request paramter
            WebRequestMultipartFormDataSupport mp = new WebRequestMultipartFormDataSupport();
            mp.PrepareWebRequest(request);
            
            // 3. go ahead
            return request;
        }

        public virtual WebRequestStream GetRequestStreamMultiPartUpload(WebRequest request, String fileName, long fileSize)
        {
            // generate mp support
            WebRequestMultipartFormDataSupport mp = new WebRequestMultipartFormDataSupport();

            // set the right size
            fileSize = fileSize + mp.GetHeaderFooterSize(fileName);

            // set the streaming buffering
#if !WINDOWS_PHONE
            if (fileSize > 0)
            {                
                // set the maximum content length size
                request.ContentLength = fileSize;
            }
#endif
            // get the stream
            WebRequestStream stream = GetRequestStream(request, fileSize);

            // prepare the stream            
            mp.PrepareRequestStream(stream, fileName);

            // go ahead
            return stream;
        }

        #endregion                                     
    }
}
