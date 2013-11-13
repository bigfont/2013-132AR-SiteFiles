using System;
using System.IO;
using System.Threading;

using AppLimit.CloudComputing.SharpBox.DropBox;

namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// The class CloudStorage implements all needed methods to get access
    /// to a supported cloud storage infrastructure. The following vendors
    /// are currently supported:
    /// 
    ///  - DropBox
    ///  
    /// </summary>
    public class CloudStorage : ICloudStorageProvider
    {
        private ICloudStorageProvider _provider;

        /// <summary>
        /// True when a the connection to the Cloudstorage will be established
        /// </summary>
        public Boolean IsOpened { get; private set; }

        /// <summary>
        /// Calling this method with vendor specific configuration settings and credentials
        /// to get access to the cloud storage.
        /// </summary>
        /// <param name="configuration">Vendor specific configuration of the cloud storage</param>
        /// <param name="credentials">Vendor specific credentials for authorization</param>
        /// <returns>Return false when something went wrong!</returns>
        public Boolean Open(ICloudStorageConfiguration configuration, ICloudeStorageCredentials credentials)
        {
            // check state
            if (IsOpened)
                return true;

            // check which kind of storage do we support
            if (!(configuration is DropBoxConfiguration))
                return false;
            
            _provider = new DropBoxStorageProvider();
            IsOpened = _provider.Open(configuration, credentials);

            return IsOpened;
        }

        /// <summary>
        /// This method will close the connection to the cloud storage system
        /// </summary>
        public void Close()
        {
            if (_provider == null)
                return;

            _provider.Close();

            IsOpened = false;
        }

        /// <summary>
        /// This method returns access to the root folder of the storage system
        /// </summary>
        /// <returns>Reference to the root folder of the storage system</returns>
        public ICloudDirectoryEntry GetRoot()
        {
            return _provider == null ? null : _provider.GetRoot();
        }

        /// <summary>
        /// This method creates a child folder in the give folder
        /// of the cloud storage
        /// </summary>
        /// <param name="name">Name of the new folder</param>
        /// <param name="parent">Parent folder</param>
        /// <returns>Reference to the created folder</returns>
        public ICloudDirectoryEntry CreateFolder(String name, ICloudDirectoryEntry parent)
        {
            return _provider == null ? null : _provider.CreateFolder(name, parent);
        }

        /// <summary>
        /// This method removes a file or a directory from 
        /// the cloude storage
        /// </summary>
        /// <param name="fsentry">Reference to the filesystem object which has to be removed</param>
        /// <returns>Returns true or false</returns>
        public bool DeleteFileSystemEntry(ICloudFileSystemEntry fsentry)
        {
            return _provider == null ? false : _provider.DeleteFileSystemEntry(fsentry);
        }


        /// <summary>
        /// This method moves a file or a directory from its current
        /// location to a new onw
        /// </summary>
        /// <param name="fsentry">Filesystem object which has to be moved</param>
        /// <param name="newParent">The new location of the targeted filesystem object</param>
        /// <returns></returns>
        public bool MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            return _provider == null ? false : _provider.MoveFileSystemEntry(fsentry, newParent);
        }
   
        /// <summary> 
        /// This method creates a new file object in the cloud storage. Use the GetContentStream method to 
        /// get a .net stream which usable in the same way then local stream are usable.
        /// </summary>
        /// <param name="parent">Link to the parent container, null means the root directory</param>
        /// <param name="Name">The name of the targeted file</param>        
        /// <returns></returns>
        public ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String Name)
        {            
            // pass through the provider
            return _provider == null ? null : _provider.CreateFile(parent, Name);
        }        
    }
}
