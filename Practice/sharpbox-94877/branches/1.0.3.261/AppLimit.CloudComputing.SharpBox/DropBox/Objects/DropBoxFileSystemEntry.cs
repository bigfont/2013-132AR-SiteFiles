using System;
using System.IO;

using AppLimit.CloudComputing.SharpBox.DropBox.Logic;
using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
    internal class DropBoxFileSystemEntry : ICloudFileSystemEntry
    {
        protected readonly DropBoxApplication AssociatedApplication;
        protected readonly DropBoxSession AssociatedSession;
        public String DropBoxPath { get; protected set; }
        private DropBoxDirectoryEntry _parent;
        private JsonHelper _lastJsonMessage;

        public DropBoxFileSystemEntry(DropBoxApplication associatedApplication, DropBoxSession associatedSession, String name, ICloudDirectoryEntry parentDirectory)
        {
            // store the application of this object 
            AssociatedApplication = associatedApplication;
            _parent = parentDirectory as DropBoxDirectoryEntry;

            // store the session
            AssociatedSession = associatedSession;

            // set the name
            Name = name;

            // set the dropbox path
            if (_parent == null)
                DropBoxPath = "/";
            else
                DropBoxPath = _parent.DropBoxPath + "/" + name;
        }

        public DropBoxFileSystemEntry(DropBoxApplication associatedApplication, DropBoxSession associatedSession, ICloudDirectoryEntry parent, String jsonText)
            : this(associatedApplication, associatedSession, "", parent)
        {            
            // build the entry
            if (jsonText != null)
                BuildEntry(jsonText);
        }

        public string Name { get; protected set; }

        public long Length { get; protected set; }

        public DateTime Modified { get; protected set;  }        

        public ICloudDirectoryEntry Parent
        {
            get { return _parent; }

            set { _parent = value as DropBoxDirectoryEntry; }
        }

        public Boolean BuildEntry(String json)
        {
            // parse the jason object
			var jh = new JsonHelper();
			if ( jh.ParseJsonMessage(json) )                        
            	// call the function
            	return BuildEntry(jh);
			else
				return false;
        }

        protected virtual Boolean BuildEntry(JsonHelper jh)
        {
            /*
             *  "revision": 29251,
                "thumb_exists": false,
                "bytes": 37941660,
                "modified": "Tue, 01 Jun 2010 14:45:09 +0000",
                "path": "/Public/2010_06_01 15_53_48_336.nvl",
                "is_dir": false,
                "icon": "page_white",
                "mime_type": "application/octet-stream",
                "size": "36.2MB"
             * */

            // set the dropbox path
            DropBoxPath = jh.GetProperty("path");
			
			// set the size
            Length = Convert.ToInt64(jh.GetProperty("bytes"));            

            // set the modified time
            Modified = jh.GetDateTimeProperty("modified");
            
            // build the displayname
            var arr = DropBoxPath.Split('/');
            Name = arr.Length > 0 ? arr[arr.Length - 1] : DropBoxPath;
			
            // set this as last json message
            _lastJsonMessage = jh;

			// go ahead
			return true;
        }

        public String GetDropBoxPath()
        {
            return DropBoxPath;
        }

        public Stream GetContentStream(System.IO.FileAccess access)
        {
        	if (access == FileAccess.ReadWrite)
                throw new NotImplementedException();
        	
			return AssociatedApplication.GetContentStream(AssociatedSession, this.Parent, this.Name, access);
        }

        public String GetPropertyValue(String key)
        {
            return _lastJsonMessage.GetProperty(key);
        }
    }
}
