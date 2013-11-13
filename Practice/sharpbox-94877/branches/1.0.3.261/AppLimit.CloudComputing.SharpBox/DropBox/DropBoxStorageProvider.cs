using System;
using System.IO;
using System.Collections.Generic;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.Common;
using AppLimit.CloudComputing.SharpBox.DropBox.Logic;
using AppLimit.CloudComputing.SharpBox.DropBox.Objects;

namespace AppLimit.CloudComputing.SharpBox.DropBox
{
    internal class DropBoxStorageProvider : ICloudStorageProvider
    {
        DropBoxApplication _application;
        DropBoxSession _session;

        #region ICloudStorageProvider Members

        public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials)
        {
            // get the right references
            var dbService = new DropBoxService();
            var cred = credentials as DropBoxBaseCredentials;

            // get the application
            _application = dbService.GetApplication(cred.ConsumerKey, cred.ConsumerSecret);

            // check if we have a user login
            if (credentials is DropBoxCredentials)
            {
                var userCred = cred as DropBoxCredentials;

                // get the session
                _session = _application.Authorize(userCred.UserName, userCred.Password);
            }

            else if (credentials is DropBoxCredentialsToken)
            {
                // get the toke cred
                var tokeCred = cred as DropBoxCredentialsToken;

                // get the access token
                DropBoxToken token = tokeCred.AccessToken as DropBoxToken;
                if (token == null)
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidCredentialsOrConfiguration);

                // get the session
                _session = _application.Authorize(token);
            }
            else
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidCredentialsOrConfiguration);

            if (_session == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidCredentialsOrConfiguration);

            // return the token
            return new DropBoxToken(_session.AccessToken);
        }

        public void Close()
        {
            _application.Close(_session);
        }

        public ICloudDirectoryEntry GetRoot()
        {
            return _application.GetRoot(_session);
        }

        public ICloudFileSystemEntry GetFileSystemObject(String path, ICloudDirectoryEntry parent)
        {
            return _application.GetFileSystemObject(path, parent, _session);
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

        public bool RenameFileSystemEntry(ICloudFileSystemEntry fsentry, string newName)
        {
            return _application.RenameItem(_session, fsentry, newName);
        }

        public ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String Name)
        {
            return _application.CreateFile(_session, parent, Name);

        }

        public Uri GetFileSystemObjectUrl(String path, ICloudDirectoryEntry parent)
        {
            return _application.GetFileSystemObjectUrl(path, parent, _session);
        }
               
        public void StoreToken(List<string> tokendata, ICloudStorageAccessToken token)
        {
            var dropboxToken = token as DropBoxToken;
            tokendata.Add(dropboxToken.TokenSecret);
            tokendata.Add(dropboxToken.TokenKey);
        }        

        public ICloudStorageAccessToken LoadToken(List<string> tokendata)
        {
            String type = tokendata[0];

            if (!type.Equals(typeof(DropBoxToken).ToString()))
                throw new InvalidCastException("Token type not supported through this provider");
                
            var tokenSecret = tokendata[1];
            var tokenKey = tokendata[2];

            return new DropBoxToken(tokenKey, tokenSecret);                            
        }

        #endregion
    }
}
