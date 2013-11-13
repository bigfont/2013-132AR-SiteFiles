using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.Common.IO;
using System.IO;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider
{
    internal class GenericStorageProvider : ICloudStorageProvider
    {
        protected IStorageProviderService _Service;
        protected IStorageProviderSession _Session;

        public GenericStorageProvider(IStorageProviderService service)
        {
            _Service = service;
        }

        #region ICloudStorageProvider Members

        public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials)
        {
            // Verify the compatibility of the credentials
            if ( !_Service.VerifyCredentialType(credentials))
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidCredentialsOrConfiguration);

            // create a new session
            _Session = _Service.CreateSession(credentials, configuration);  

            // check the session
            if (_Session == null)
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidCredentialsOrConfiguration);

            // return the accesstoken token
            return _Session.SessionToken;                            
        }

        public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudStorageAccessToken token)
        {            
            // create a new session
            _Session = _Service.CreateSession(token, configuration);

            // check the session
            if (_Session == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidCredentialsOrConfiguration);

            // return the accesstoken token
            return _Session.SessionToken;    
        }

        public void Close()
        {
            // close the session
            _Service.CloseSession(_Session);

            // remove reference
            _Session = null;
        }

        public ICloudDirectoryEntry GetRoot()
        {
            return _Service.RequestResource(_Session, "/", null) as ICloudDirectoryEntry;
        }

        public ICloudFileSystemEntry GetFileSystemObject(string path, ICloudDirectoryEntry parent)
        {                        
            /*
             * This section generates for every higher object the object tree
             */
            PathHelper ph = new PathHelper(path);
            String[] elements = ph.GetPathElements();

            // create the virtual root
            ICloudDirectoryEntry    current = parent;            

            // build the root
            if (current == null)
            {
                current = GetRoot();
            }

            // check if we request only the root
            if (path.Equals("/"))
                return current;

            // create the path tree
            for (int i = 0; i <= elements.Length - 1; i++)
            {
                String elem = elements[i];

                if (i == elements.Length - 1)
                {
                    // get requested object 
                    ICloudFileSystemEntry requestedObject = _Service.RequestResource(_Session, elem, current);

                    // go ahead on error
                    if (requestedObject == null)
                        return null;
                    else
                        // go ahead
                        return requestedObject;
                }
                else
                {
                    try
                    {
                        // try to get the child
                        current = current.GetChild(elem) as ICloudDirectoryEntry;
                    } catch(SharpBoxException e)
                    {
                        // if not found, create a virtual one
                        if (e.ErrorCode == SharpBoxErrorCodes.ErrorFileNotFound)
                            current = _Service.CreateDirectoryEntry(_Session, elem, current);
                        else
                            throw e;
                    }                                           
                }                
            }

            // looks like an error
            return null;
        }

        public ICloudDirectoryEntry CreateFolder(string name, ICloudDirectoryEntry parent)
        {
            // solve the parent issue
            if (parent == null)
            {
                parent = GetRoot();

                if (parent == null)
                    return null;
            }
            
            // double check if the folder which has to be created 
            // is in the folder
            try
            {
                var childDir = parent.GetChild(name);
                if (childDir != null)
                    return childDir as ICloudDirectoryEntry;
            }
            catch (SharpBoxException e)
            {
                if (e.ErrorCode != SharpBoxErrorCodes.ErrorFileNotFound)
                    throw;
            }
            
            // request the object
            var res = _Service.CreateResource(_Session, name, parent);            
            if (res == null )
                return null;
            
            // go ahead
            return res as ICloudDirectoryEntry;             
        }

        public bool DeleteFileSystemEntry(ICloudFileSystemEntry fsentry)
        {
            return _Service.DeleteResource(_Session, fsentry);    
        }

        public bool MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            return _Service.MoveResource(_Session, fsentry, newParent);
        }

        public bool RenameFileSystemEntry(ICloudFileSystemEntry fsentry, string newName)
        {
            // save the old name
            String oldName = fsentry.Name;
                        
            // rename the resource
            if (_Service.RenameResource(_Session, fsentry, newName))
            {
                // get the parent
                BaseDirectoryEntry p = fsentry.Parent as BaseDirectoryEntry;

                // remove the old childname
                p.RemoveChildByName(oldName);

                // readd the child
                p.AddChild(fsentry as BaseFileEntry);

                // go ahead
                return true;
            }
            else
                return false;            
        }

        public ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, string name)
        {
            // build the parent
            if (parent == null)
                parent = GetRoot();

            // build the file entry
            var newEntry = _Service.CreateFileSystemEntry(_Session, name, parent);
            return newEntry;
        }

        public Uri GetFileSystemObjectUrl(string path, ICloudDirectoryEntry parent)
        {
            String url = _Service.GetResourceUrl(_Session, parent, path);            
            return new Uri(url);
        }

        public String GetFileSystemObjectPath(ICloudDirectoryEntry fsObject)
        {
            return GenericHelper.GetResourcePath(fsObject);
        }

        public void StoreToken(List<string> tokendata, ICloudStorageAccessToken token)
        {
            _Service.StoreToken(_Session, tokendata, token);
        }

        public ICloudStorageAccessToken LoadToken(List<string> tokendata)
        {
            return _Service.LoadToken(_Session, tokendata);
        }      

        public ICloudStorageAccessToken CurrentAccessToken
        {
            get { return _Session == null ? null : _Session.SessionToken; }
        }

        #endregion
    }
}
