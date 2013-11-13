using System;
using System.Collections.Generic;

namespace AppLimit.CloudComputing.SharpBox
{
    internal interface ICloudStorageProvider
    {        
        ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials);        
        
        void Close();

        ICloudDirectoryEntry GetRoot();

        ICloudFileSystemEntry GetFileSystemObject(String path, ICloudDirectoryEntry parent);

        ICloudDirectoryEntry CreateFolder(String name, ICloudDirectoryEntry parent);

        Boolean DeleteFileSystemEntry(ICloudFileSystemEntry fsentry);

        Boolean MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent);

        Boolean RenameFileSystemEntry(ICloudFileSystemEntry fsentry, String newName);

        ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String name);

        Uri GetFileSystemObjectUrl(String path, ICloudDirectoryEntry parent);

        void StoreToken(List<string> tokendata, ICloudStorageAccessToken token);

        ICloudStorageAccessToken LoadToken(List<string> tokendata);
    }
}
