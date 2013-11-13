using System;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox
{
    internal interface ICloudStorageProvider
    {
        Boolean Open(ICloudStorageConfiguration configuration, ICloudeStorageCredentials credentials);
        
        void Close();

        ICloudDirectoryEntry GetRoot();

        ICloudDirectoryEntry CreateFolder(String name, ICloudDirectoryEntry parent);

        Boolean DeleteFileSystemEntry(ICloudFileSystemEntry fsentry);

        Boolean MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent);       

        ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String Name);        
    }
}
