using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox
{
    public interface ICloudStorageAsyncInterface
    {
        IAsyncResult BeginOpenRequest(AsyncCallback callback, ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials);
        ICloudStorageAccessToken EndOpenRequest(IAsyncResult asyncResult);
        
        IAsyncResult BeginCreateFileRequest(AsyncCallback callback, ICloudDirectoryEntry parent, String Name);
        ICloudFileSystemEntry EndCreateFileRequest(IAsyncResult asyncResult);
    }
}
