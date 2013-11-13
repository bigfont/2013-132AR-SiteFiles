using System;
using System.IO;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.DropBox.Logic;
using AppLimit.CloudComputing.SharpBox.DropBox.Objects;

namespace AppLimit.CloudComputing.SharpBox.DropBox
{
  internal class DropBoxStorageProvider : ICloudStorageProvider
  {
    DropBoxApplication _application;
    DropBoxSession _session;

    #region ICloudStorageProvider Members

    public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudeStorageCredentials credentials)
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
    
    #endregion
  }
}
