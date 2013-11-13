using System;
using System.Collections.Generic;
using System.IO;

using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxFileStream : MemoryStream
    {
        private readonly DropBoxSession _session;
        private readonly String _url;
        private readonly String _file;        
        private readonly FileAccess _access;
       
        public DropBoxFileStream(DropBoxSession sesssion, String url, String file, FileAccess access)
        {
            _session = sesssion;
            _url = url;
            _file = file;            
            _access = access;

            if (_access == FileAccess.Read)
                DownloadData();
        }

        public override void Close()
        {
            // flush everything
            base.Flush();

            // close the stream
            base.Close();

            // upload the file when we have a write mode
            if (_access == FileAccess.Write)
            {
                // build the data stream                             
                var data = new Dictionary<string,Byte[]>();
                data.Add(_file, ToArray());

                var param = new Dictionary<string, string>();
                param.Add("file", _file);

                _session.PostRessourcByUrl(_url, param, data);
            }            
        }

        private void DownloadData()
        {
            Stream data = _session.RequestRessourceStreamByUrl(_url + "/" + EncodingHelper.UTF8Encode(_file));
            StreamHelper.CopyStreamData(data, this);

            if (Length > 0 && CanSeek)
                Position = 0;
        }
    }
}
