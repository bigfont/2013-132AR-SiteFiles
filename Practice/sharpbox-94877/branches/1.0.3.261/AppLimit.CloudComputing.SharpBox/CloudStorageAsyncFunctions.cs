using System;
using System.IO;
using System.Threading;

using AppLimit.CloudComputing.SharpBox.DropBox;
using AppLimit.CloudComputing.SharpBox.Common;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox
{
    public delegate ICloudFileSystemEntry CreateFileDelegate(ICloudDirectoryEntry parent, String Name);
    public delegate ICloudStorageAccessToken OpenDelegate(ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials);

    public partial class CloudStorage
    {             
        #region Async Functions

        internal class OpenRequest : AsyncObjectRequest 
        {
            public ICloudStorageConfiguration config;
            public ICloudStorageCredentials credentials;
            public ICloudStorageAccessToken token;            
        }

        public void OpenRequestCallback(object state)
        {
            // cast the request 
            OpenRequest req = state as OpenRequest;

            try
            {
                // perform the request
                req.token = Open(req.config, req.credentials);
            }
            catch (Exception e)
            {
                // failure to login
                var openRequest = req.result.AsyncState as OpenRequest;
                openRequest.token = null;
                openRequest.errorReason = e;
            }

            // call the async callback
            req.callback(req.result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="configuration"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public IAsyncResult BeginOpenRequest(AsyncCallback callback, ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials)
        {
            // build the request data structure
            OpenRequest request = new OpenRequest();
            request.callback = callback;
            request.result = new AsyncResultEx(request);
            request.config = configuration;
            request.credentials = credentials;
           
            // add to threadpool
            ThreadPool.QueueUserWorkItem(OpenRequestCallback, request);
            
            // return the result
            return request.result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public ICloudStorageAccessToken EndOpenRequest(IAsyncResult asyncResult)
        {
            OpenRequest req = asyncResult.AsyncState as OpenRequest;
            return req.token;   
        }

        /// <summary>
        /// This method starts the async call of creating a file
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="parent"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IAsyncResult BeginCreateFileRequest(AsyncCallback callback, ICloudDirectoryEntry parent, String Name)
        {
            CreateFileDelegate del = new CreateFileDelegate(CreateFile);
            return del.BeginInvoke(parent, Name, callback, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public ICloudFileSystemEntry EndCreateFileRequest(IAsyncResult asyncResult)
        {
            /*AsyncResult res = asyncResult as AsyncResult;
            CreateFileDelegate del = res.AsyncDelegate as CreateFileDelegate;

            return del.EndInvoke(asyncResult);*/

            return null;
        }

        #endregion
    }
}
