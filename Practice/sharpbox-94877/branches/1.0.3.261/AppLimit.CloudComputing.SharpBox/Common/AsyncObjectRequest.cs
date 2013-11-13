using System;
using System.Net;

namespace AppLimit.CloudComputing.SharpBox.Common
{
    public class AsyncObjectRequest
    {
        public AsyncCallback callback;
        public IAsyncResult result;
        public Exception errorReason;   
    }
}
