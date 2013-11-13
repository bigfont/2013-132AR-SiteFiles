using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxFileStream : MemoryStream
    {
        private DropBoxSession _session;
        private String _url;
        private String _file;        
        private FileAccess _access;
       
        public DropBoxFileStream(DropBoxSession sesssion, String url, String File, FileAccess access)
        {
            _session = sesssion;
            _url = url;
            _file = File;            
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
                data.Add(_file, this.ToArray());

                Dictionary<String, String> param = new Dictionary<string, string>();
                param.Add("file", _file);

                _session.PostRessourcByUrl(_url, param, data);
            }            
        }

        private void DownloadData()
        {
            Stream data = _session.RequestRessourceStreamByUrl(_url + "/" + _file);
            StreamHelper.CopyStreamData(data, this);

            if ( this.Length > 0 && this.CanSeek)
                this.Position = 0;
        }
    }
}
