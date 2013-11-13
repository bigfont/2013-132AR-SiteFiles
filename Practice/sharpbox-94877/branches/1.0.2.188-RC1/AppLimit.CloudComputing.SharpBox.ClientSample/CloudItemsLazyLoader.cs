using AppLimit.Common.Toolkit.Controller;

namespace AppLimit.CloudComputing.SharpBox.ClientSample
{
    public class CloudItemsLazyLoader : ICommonLazyLoadProvider
    {
        #region ILazyLoadProvider Members

        public bool IsLazyLoadNeeded(AppLimit.Common.Objects.ICommonContainer target)
        {
            var folder = target as CloudFolderShim;
            if (folder != null) 
                return (folder.Childs.Count == 0);
            return false;
        }

        public bool LoadChildObjects(AppLimit.Common.Objects.ICommonContainer target)
        {
            var folder = target as CloudFolderShim;
            if (folder != null) 
                return folder.LoadChildsFromCloud();
            return false;
        }
        
        public bool RemoveChildObjects(AppLimit.Common.Objects.ICommonContainer target)
        {
            var folder = target as CloudFolderShim;
            if (folder != null)
                folder.Childs.Clear();

            return true;
        }

        #endregion
    }
}
