using System;

namespace AppLimit.CloudComputing.SharpBox
{
    internal interface ICloudStorageProvider
    {
        ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudeStorageCredentials credentials);        
        
        void Close();

        ICloudDirectoryEntry GetRoot();

        ICloudDirectoryEntry CreateFolder(String name, ICloudDirectoryEntry parent);

        Boolean DeleteFileSystemEntry(ICloudFileSystemEntry fsentry);

        Boolean MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent);

        Boolean RenameFileSystemEntry(ICloudFileSystemEntry fsentry, String newName);

        ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String name);        
    }
}
