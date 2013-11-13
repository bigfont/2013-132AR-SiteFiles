using System;
using System.Net;

namespace AppLimit.CloudComputing.SharpBox.Common
{
    public class AsyncResultEx : IAsyncResult
    {
        public AsyncResultEx(Object AsyncState)
        {
            this.AsyncState = AsyncState;
        }

        public object AsyncState
        {
            get;
            private set;
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { throw new NotImplementedException(); }
        }

        public bool CompletedSynchronously
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCompleted
        {
            get { throw new NotImplementedException(); }
        }
    }
}
