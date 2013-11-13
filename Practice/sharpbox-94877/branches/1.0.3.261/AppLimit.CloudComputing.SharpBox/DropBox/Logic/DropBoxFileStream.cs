using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxFileStream : Stream
    {
        private readonly DropBoxSession _session;
        private readonly String _url;
        private readonly String _file;        
        private readonly FileAccess _access;

        private MemoryStream            _writeCache;
        private readonly Stream         _dropboxNetStream;

        public DropBoxFileStream(DropBoxSession sesssion, String url, String file, FileAccess access)
        {
            _session = sesssion;
            _url = url;
            _file = file;            
            _access = access;            

            if (_access == FileAccess.Read)
                _dropboxNetStream = _session.RequestResourceStreamByUrl(_url + "/" + EncodingHelper.UTF8Encode(_file));
            else if (_access == FileAccess.Write)
                _writeCache = new MemoryStream();
        }

        public override void Close()
        {
            // flush everything
            Flush();

            // close the stream
            base.Close();

            // upload the file when we have a write mode
            if (_access == FileAccess.Write)
            {
                // build the data stream                             
                var data = new Dictionary<string,Byte[]>();
                data.Add(_file, _writeCache.ToArray());

                var param = new Dictionary<string, string>();
                param.Add("file", _file);

                _session.PostResourceByUrl(_url, param, data);
            }            
        }        

        public override bool CanRead
        {
            get { return (_access == FileAccess.Read || _access == FileAccess.ReadWrite); }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return (_access == FileAccess.Write || _access == FileAccess.ReadWrite); }
        }

        public override void Flush()
        {
            if (CanWrite)
                _writeCache.Flush();
        }

        public override long Length
        {
            get 
            {
                if (CanWrite)
                    return _writeCache.Length;
                else
                    return _dropboxNetStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                if (CanWrite)
                    return _writeCache.Position;
                else
                    return _dropboxNetStream.Position;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (CanRead)
                return _dropboxNetStream.Read(buffer, offset, count);
            else
                throw new InvalidOperationException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            if (CanWrite)
                _writeCache.SetLength(value);
            else
                throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite)
                _writeCache.Write(buffer, offset, count);
            else
                throw new InvalidOperationException();
        }
    }
}
