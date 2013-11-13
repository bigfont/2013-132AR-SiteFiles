using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace AppLimit.CloudComputing.SharpBox.Common.Net.Web
{
    internal delegate void WebRequestStreamDisposeOperation(params object[] args);

    internal class WebRequestStream : Stream, IDisposable
    {
        private Stream              _requestStream;
        private WebRequest          _request;
        private WebRequestService   _service;
        private Stack<WebRequestStreamDisposeOperation> _PostDisposeOperations;
        private Stack<WebRequestStreamDisposeOperation> _PreDisposeOperations;
        private Dictionary<WebRequestStreamDisposeOperation, object[]> _DisposeOperationsParams;

        public WebRequestStream(Stream srcStream, WebRequest request, WebRequestService service)
        {
            _requestStream = srcStream;
            _request = request;
            _service = service;
            _PostDisposeOperations = new Stack<WebRequestStreamDisposeOperation>();
            _PreDisposeOperations = new Stack<WebRequestStreamDisposeOperation>();
            _DisposeOperationsParams = new Dictionary<WebRequestStreamDisposeOperation, object[]>();
        }

        public void PushPostDisposeOperation(WebRequestStreamDisposeOperation opp, params object[] args)
        {
            _PostDisposeOperations.Push(opp);

            if ( args != null )
                _DisposeOperationsParams.Add(opp, args);
        }

        public void PushPreDisposeOperation(WebRequestStreamDisposeOperation opp, params object[] args)
        {
            _PreDisposeOperations.Push(opp);

            if (args != null)
                _DisposeOperationsParams.Add(opp, args);
        }

        public override bool CanRead
        {
            get { return _requestStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _requestStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _requestStream.CanWrite; }
        }

        public override void Flush()
        {
            _requestStream.Flush();
        }

        public override long Length
        {
            get { return _requestStream.Length; }
        }

        public override long Position
        {
            get
            {
                return _requestStream.Position;
            }
            set
            {
                _requestStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _requestStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _requestStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _requestStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _requestStream.Write(buffer, offset, count);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {                       
            // dispose our pre opps
            PerformeDisposeOperations(_PreDisposeOperations);

            // to all dispose stuff from base
            base.Dispose();

            // dispose our post opps
            PerformeDisposeOperations(_PostDisposeOperations);
        }

        private void PerformeDisposeOperations(Stack<WebRequestStreamDisposeOperation> stack)
        {
            // dispose our opps
            while (stack.Count > 0)
            {
                // pop the opp
                WebRequestStreamDisposeOperation dop = stack.Pop();

                // get the args
                object[] args = null;
                _DisposeOperationsParams.TryGetValue(dop, out args);

                // perform opp
                dop(args);
            }
        }

        #endregion
    }
}
