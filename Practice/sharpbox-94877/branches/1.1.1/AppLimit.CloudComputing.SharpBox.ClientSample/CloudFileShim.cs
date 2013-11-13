using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Objects;

namespace AppLimit.CloudComputing.SharpBox.ClientSample
{
    public class CloudFileShim : ICommonContainerObject 
    {
        public readonly ICloudFileSystemEntry cloudFSEntry;        
        private CloudFolderShim _parent;

        public CloudFileShim(ICloudFileSystemEntry entry, CloudFolderShim parent)
        {
            cloudFSEntry = entry;
            _parent = parent;            
        }
    
        #region ICommonObject Members
      
        public ICommonContainer Parent
        {
            get
            {   
                return _parent;
            }

            set
            {
                _parent = (CloudFolderShim)value;
            }
        }
       
        public string Tag
        {
            get
            {
                return cloudFSEntry.Name.Length == 0 ? "Root" : cloudFSEntry.Name;
            }
        }

        #endregion
    }
}
