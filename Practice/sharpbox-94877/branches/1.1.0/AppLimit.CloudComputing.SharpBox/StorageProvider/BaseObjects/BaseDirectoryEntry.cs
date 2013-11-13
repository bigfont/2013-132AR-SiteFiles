using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects
{
    internal class BaseDirectoryEntry : BaseFileEntry, ICloudDirectoryEntry
    {
        private Dictionary<String, ICloudFileSystemEntry>   _subDirectories         = new Dictionary<string,ICloudFileSystemEntry>(StringComparer.OrdinalIgnoreCase);
        private List<ICloudFileSystemEntry>                 _subDirectoriesByIndex  = new List<ICloudFileSystemEntry>();

        public BaseDirectoryEntry(String Name, long Length, DateTime Modified, IStorageProviderService service, IStorageProviderSession session)
            : base(Name, Length, Modified, service, session)
        { 
        }

        #region ICloudDirectoryEntry Members

        public ICloudFileSystemEntry GetChild(string name)
        {
            return GetChild(name, true);
        }

        public ICloudFileSystemEntry GetChild(string name, Boolean bThrowException)
        { 
            // refresh this item to ensure that the parents
            // are valud
            _service.RefreshResource(_session, this);

            // query subitems
            ICloudFileSystemEntry pe;
            _subDirectories.TryGetValue(name, out pe);           
            
            // send the exception if needed
            if (pe == null && bThrowException == true)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);
            
            // go ahead
            return pe;
        }

        public ICloudFileSystemEntry GetChild(int idx)
        {
            // refresh this item to ensure that the parents
            // are valud
            _service.RefreshResource(_session, this);

            // request the information
            return idx < _subDirectoriesByIndex.Count ? _subDirectoriesByIndex[idx] : null;
        }

        public int Count
        {
            get
            {				
                // go ahead
                return _subDirectories.Count;
            }
        }

        #endregion

        #region IEnumerable<ICloudFileSystemEntry> Members       

        public IEnumerator<ICloudFileSystemEntry> GetEnumerator()
        {
            // refresh this item to ensure that the parents
            // are valud
            _service.RefreshResource(_session, this);

            // go ahead
            return _subDirectories.Values.GetEnumerator();
        }       

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        

        public void AddChild(BaseFileEntry child)
        {
            // remove child if available
            RemoveChild(child);

            // add the new child
            _subDirectories.Add(child.Name, child);
            _subDirectoriesByIndex.Add(child);

            // set the parent
            child.Parent = this;
        }

        public void AddChilds(IEnumerable<BaseFileEntry> childs)
        {
            foreach (BaseFileEntry entry in childs)
                AddChild(entry);
        }

        public void RemoveChild(BaseFileEntry child)
        {
            RemoveChildByName(child.Name);
        }

        public void RemoveChildByName(String childName)
        {
            // check for replace
            if (_subDirectories.ContainsKey(childName))
            {
                // get the old child
                ICloudFileSystemEntry oldChild = _subDirectories[childName];

                // remove it
                _subDirectories.Remove(childName);
                _subDirectoriesByIndex.Remove(oldChild);

                // remove parent link
                oldChild.Parent = null;
            }
        }

        public void ClearChilds()
        {
            _subDirectories.Clear();
            _subDirectoriesByIndex.Clear();
        }
    }
}
