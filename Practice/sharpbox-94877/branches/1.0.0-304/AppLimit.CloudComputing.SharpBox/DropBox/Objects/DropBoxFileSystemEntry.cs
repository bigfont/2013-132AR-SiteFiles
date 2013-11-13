using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                this.DropBoxPath = "/";
            else
                this.DropBoxPath = _parent.DropBoxPath + "/" + name;
        }

        public DropBoxFileSystemEntry(DropBoxApplication associatedApplication, DropBoxSession associatedSession, ICloudDirectoryEntry parent, String jsonText)
            : this(associatedApplication, associatedSession, "", parent)
        {            
            // build the entry
            if (jsonText != null)
                BuildEntry(jsonText);
        }

        public string Name { get; private set; }

        public ICloudDirectoryEntry Parent
        {
            get { return _parent; }

            set { _parent = value as DropBoxDirectoryEntry; }
        }

        public Boolean BuildEntry(String json)
        {
            // parse the jason object
			JsonHelper jh = new JsonHelper();
			if ( jh.ParseJsonMessage(json) )                        
            	// call the function
            	return BuildEntry(jh);
			else
				return false;
        }

        protected virtual Boolean BuildEntry(JsonHelper jh)
        {
            // set the dropbox path
            DropBoxPath = jh.GetProperty("path");
						
            // build the displayname
            var arr = DropBoxPath.Split('/');
            Name = arr.Length > 0 ? arr[arr.Length - 1] : DropBoxPath;
			
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
            else
                return AssociatedApplication.GetContentStream(AssociatedSession, this.Parent, this.Name, access);           
        }
    }
}
