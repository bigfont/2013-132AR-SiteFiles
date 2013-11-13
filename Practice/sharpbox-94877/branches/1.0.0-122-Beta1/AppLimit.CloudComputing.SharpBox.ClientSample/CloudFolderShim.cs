using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.Common.Objects;
using AppLimit.CloudComputing.SharpBox;

namespace AppLimit.CloudComputing.SharpBox.ClientSample 
{
    public class CloudFolderShim : CloudFileShim, ICommonContainer
    {
        private readonly List<ICommonContainerObject> _childs;

        public CloudFolderShim( ICloudDirectoryEntry entry, CloudFolderShim parent )
            : base(entry, parent)
        {
            _childs = new List<ICommonContainerObject>();
            
        }

        public Boolean LoadChildsFromCloud()
        {
            var dentry = cloudFSEntry as ICloudDirectoryEntry;

            var enumerator = dentry.GetEnumerator();
            if (enumerator == null)
                return false;
            else
            {
                enumerator.Reset();
                enumerator.MoveNext();
                while (enumerator.Current != null)
                {
                    if ( enumerator.Current is ICloudDirectoryEntry )
                        _childs.Add(new CloudFolderShim(enumerator.Current as ICloudDirectoryEntry, this));
                    else
                        _childs.Add(new CloudFileShim(enumerator.Current,this));    
                        
                    enumerator.MoveNext();
                }

                return true;
            }
        }
        
        #region ICommonContainer Members

        public List<ICommonContainerObject> Childs
        {
            get
            {
                return _childs;
            }
        }
       
        public ICommonContainerObject GetChildByTag(string Tag)
        {
            return _childs.Find(fc => fc.Tag.Equals(Tag));
        }

        #endregion
    }
}
