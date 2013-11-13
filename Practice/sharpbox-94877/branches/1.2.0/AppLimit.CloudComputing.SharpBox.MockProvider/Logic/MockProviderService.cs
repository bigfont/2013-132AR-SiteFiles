using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.MockProvider.Model;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.MockProvider.Logic
{
    /// <summary>
    /// This derived class is a special implementation of the StorageProvider services 
    /// interface which is responsable for all interaction with the remote service. The mock 
    /// service works as follows:
    /// - Virtual FileSystem is /
    ///     - Data
    ///         - TestFolder1
    ///         - TestFolder2
    ///     - Date2
    ///         - TestFolder3
    ///             - File.rnd
    ///
    /// - It's possible to upload data and create diretories, just in memory
    /// 
    /// </summary>
    internal class MockProviderService : GenericStorageProviderService
    {
        /// <summary>
        /// Verify credentialtype has to be implemented to verify if the given credentials are
        /// compatible with this service! Our MockProvider only accepts MockProviderCredentials.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public override bool VerifyAccessTokenType(ICloudStorageAccessToken token)        
        {
            if (token is MockProviderCredentials)
                return true;
            else
                return false;            
        }

        /// <summary>
        /// The CreateSession method has to establish an authenticated connection with the cloud 
        /// storage service and returns an instance of a type which implements the 
        /// IStorageProviderSession interface. The MockProvider just returns an instance of the
        /// MockProviderSession classe.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public override IStorageProviderSession CreateSession(ICloudStorageAccessToken token, ICloudStorageConfiguration configuration)
        {
            // return the session {Real session handlers will store the configuration and the created accesstoken in the session)
            return new MockProviderSession(token as MockProviderCredentials, this, configuration as MockPoviderConfiguration);
        }
        
        /// <summary>
        /// The request resource interface is reponsable to communicate with the cloud storage service to 
        /// download all meta data of a specific resource which is adressed via Name and parent container. 
        /// The mock provider works only in memory so only the existing childs will be returned.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="Name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override ICloudFileSystemEntry RequestResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            // check if the root element is request
            if (Name.Equals("/"))
                return ((MockProviderSession)session).GetVirtualRoot();

            return parent.GetChild(Name);            
        }

        /// <summary>
        /// This method is reponsable for refreshing an resource in the memory meta data cache against the 
        /// real existing resources on the cloud storage provider. The Mock Provider is working on memory 
        /// only, so nothing todo
        /// </summary>
        /// <param name="session"></param>
        /// <param name="resource"></param>
        public override void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource)
        {            
        }

        /// <summary>
        /// This method is called from the runtime when a resource has to be created in the cloud storage 
        /// provider. This means not that data will be uploaded, only the meta data information will be 
        /// generated. In our example only memory operations are implemented so we create the resource in 
        /// our memory model.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="Name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override ICloudFileSystemEntry CreateResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            return GenericStorageProviderFactory.CreateDirectoryEntry(session, Name, parent);            
        }
        
        /// <summary>
        /// This method is called when a specific resource has to be removed from the cloud storage service. 
        /// The Mock Provider only implements in memory actions so we remove the resource only from our virtual 
        /// filesystem in memory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public override bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            // remove from memory tree
            GenericStorageProviderFactory.DeleteFileSystemEntry(session, entry);
            
            // go ahead
            return true;            
        }

        /// <summary>
        /// This method is called from the runtime if a resource has to be removed from his current 
        /// parent under a new parent. The MockProvider has to do the move operation only in his 
        /// in memory cache.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fsentry"></param>
        /// <param name="newParent"></param>
        /// <returns></returns>
        public override bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            // move the entry
            GenericStorageProviderFactory.MoveFileSystemEntry(session, fsentry, newParent);

            // go ahead
            return true;
        }

        /// <summary>
        /// This method is called to establish a download stream. In real world scenarios the stream will be a
        /// WebREquest based stream which needs to execute some task during disposal. 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fileSystemEntry"></param>
        /// <returns></returns>
        public override System.IO.Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry)
        {
            // cast the sesion
            MockProviderSession ses = (MockProviderSession)session;

            // receive the data stream from session 
            return ses.GetDataStream(fileSystemEntry);  
        }

        /// <summary>
        /// This method will be called to commit a generated stream operation. In our example only the filesize 
        /// length has to be setted as post operations scenario. 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fileSystemEntry"></param>
        /// <param name="Direction"></param>
        /// <param name="NotDisposedStream"></param>
        public override void CommitStreamOperation(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, nTransferDirection Direction, Stream NotDisposedStream)
        {
            if ( Direction == nTransferDirection.nUpload)
                GenericStorageProviderFactory.ModifyFileSystemEntryLength(fileSystemEntry, NotDisposedStream.Length);
        }

        /// <summary>
        /// This method is called to establish an upload stream. In real world scenarios the stream will be a
        /// WebREquest based stream which needs to execute some task during disposal. 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fileSystemEntry"></param>
        /// <param name="uploadSize"></param>
        /// <returns></returns>
        public override System.IO.Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, long uploadSize)
        {
            // cast the sesion
            MockProviderSession ses = (MockProviderSession)session;

            // receive the data stream from session 
            return ses.GetDataStream(fileSystemEntry);        
        }

        /// <summary>
        /// This method is called if a resource has to be renamed in the cloud storage service. Our 
        /// MockProvider needs only to rename the resource in memory
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fsentry"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public override bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, string newName)
        {
            // rename in memory
            GenericStorageProviderFactory.RenameFileSystemEntry(session, fsentry, newName);

            // go ahead
            return true;
        }
    }
}
