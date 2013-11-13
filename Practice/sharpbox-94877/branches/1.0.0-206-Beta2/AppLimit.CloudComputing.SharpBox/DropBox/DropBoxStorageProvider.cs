using System;
using System.IO;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.DropBox.Logic;

namespace AppLimit.CloudComputing.SharpBox.DropBox
{
    internal class DropBoxStorageProvider : ICloudStorageProvider
    {
        DropBoxApplication      _application;
        DropBoxSession         _session;

        #region ICloudStorageProvider Members

        public bool Open(ICloudStorageConfiguration configuration, ICloudeStorageCredentials credentials)
        {
            var cred = credentials as DropBoxCredentials;
            if (cred == null)
                return false;

            var dbService = new DropBoxService();

            _application = dbService.GetApplication(cred.ConsumerKey, cred.ComsumerSecret);
            if (_application == null)
                return false;

            _session = _application.Authorize(cred.UserName, cred.Password, !configuration.HasToShowAuthorizationProcess );
            return _session != null;
        }

        public void Close()
        {
            _application.Close(_session);
        }

        public ICloudDirectoryEntry GetRoot()
        {
            return _application.GetRoot(_session);
        }        
        
        public ICloudDirectoryEntry CreateFolder(string name, ICloudDirectoryEntry parent)
        {
            return _application.CreateFolder(_session, name, parent);
        }

        public bool DeleteFileSystemEntry(ICloudFileSystemEntry fsentry)
        {
            return _application.DeleteItem(_session, fsentry);
        }
        
        public bool MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            return _application.MoveItem(_session, fsentry, newParent);
        }

        public ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String Name)
        {
            return _application.CreateFile(_session, parent, Name); 
            
        }        

        #endregion
    }
}
