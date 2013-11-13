using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

using AppLimit.CloudComputing.SharpBox.Common.IO;
using AppLimit.CloudComputing.SharpBox.Common.Net;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web.Dav;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider
{
    internal abstract class GenericStorageProviderService : IStorageProviderService
    {        
        public abstract bool VerifyCredentialType(ICloudStorageCredentials credentials);        

        public abstract IStorageProviderSession CreateSession(ICloudStorageCredentials credentials, ICloudStorageConfiguration configuration);

                /// <summary>
        /// This method generates a session to a webdav share via access token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public virtual IStorageProviderSession CreateSession(ICloudStorageAccessToken token, ICloudStorageConfiguration configuration)
        {
            return CreateSession((token as ICloudStorageCredentials), configuration);
        }

        /// <summary>
        /// This method closes the session
        /// </summary>
        /// <param name="session"></param>        
        public virtual void CloseSession(IStorageProviderSession session)
        {
            // nothing to do here
        }

        public abstract ICloudFileSystemEntry RequestResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent);

        public abstract void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource);

        public abstract bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry);

        public abstract bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent);

        /// <summary>
        /// Writes a generic token onto the storage collection
        /// </summary>
        /// <param name="session"></param>
        /// <param name="tokendata"></param>
        /// <param name="token"></param>        
        public virtual void StoreToken(IStorageProviderSession session, List<string> tokendata, ICloudStorageAccessToken token)
        {
            if (token is GenericNetworkCredentials)
            {
                GenericNetworkCredentials creds = token as GenericNetworkCredentials;
                tokendata.Add(creds.UserName);
                tokendata.Add(creds.Password);
            }             
        }

        /// <summary>
        /// Reads the token information
        /// </summary>
        /// <param name="session"></param>
        /// <param name="tokendata"></param>
        /// <returns></returns>       
        public virtual ICloudStorageAccessToken LoadToken(IStorageProviderSession session, List<string> tokendata)
        {
            ICloudStorageAccessToken at = null;

            String type = tokendata[0];

            if (type.Equals(typeof(GenericNetworkCredentials).ToString()))
            {
                var username = tokendata[1];
                var password = tokendata[2];

                GenericNetworkCredentials bc = new GenericNetworkCredentials();
                bc.UserName = username;
                bc.Password = password;

                at = bc;
            }
#if !WINDOWS_PHONE
            else if (type.Equals(typeof(GenericCurrentCredentials).ToString()))
            {
                at = new GenericCurrentCredentials();
            }
#endif

            return at;         
        }

        /// <summary>
        /// This method build up a valid resource url
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fileSystemEntry"></param>
        /// <param name="additionalPath"></param>
        /// <returns></returns>       
        public virtual string GetResourceUrl(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, string additionalPath)
        {            
            // build the url            
            String targeturl = PathHelper.Combine(session.ServiceConfiguration.ServiceLocator.ToString(), GenericHelper.GetResourcePath(fileSystemEntry));

            // finalize the url 
            if (!(fileSystemEntry is ICloudDirectoryEntry))
                targeturl = targeturl.TrimEnd('/');

            // add the additional Path 
            if (additionalPath != null)
                targeturl = PathHelper.Combine(targeturl, additionalPath);            

            // go ahead
            return targeturl;
        }        

        public virtual void DownloadResourceContent(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, Stream targetDataStream, FileOperationProgressChanged progressCallback, Object progressContext)
        {            
            // build the download stream
            using (Stream data = CreateDownloadStream(session, fileSystemEntry))
            {                
                // copy the data                
                StreamHelper.CopyStreamData(data, targetDataStream, CloudStorage.FileStreamCopyCallback, progressCallback, fileSystemEntry, progressContext);
            }
        }

        public abstract Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry);

        public abstract Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, long uploadSize);

        public virtual void UploadResourceContent(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, Stream targetDataStream, FileOperationProgressChanged progressCallback, object progressContext)
        {
            // build the stream stream
            using (Stream data = CreateUploadStream(session, fileSystemEntry, targetDataStream.Length))
            {
                // copy the data                
                StreamHelper.CopyStreamData(targetDataStream, data, CloudStorage.FileStreamCopyCallback, progressCallback, fileSystemEntry, progressContext);

                // flush the upload stream to clean the caches
                data.Flush();                
            }
        }
                
        public abstract ICloudFileSystemEntry CreateResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent);
        
        public abstract bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, string newName);
        
        /// <summary>
        /// This method builds an object 
        /// </summary>        
        /// <param name="session"></param>
        /// <param name="Name"></param>
        /// <param name="parent"></param>        
        /// <returns></returns>
        public virtual ICloudFileSystemEntry CreateFileSystemEntry(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            // build up query url
            BaseFileEntry newObj = new BaseFileEntry(Name, 0, DateTime.Now, this, session);

            // case the parent if possible
            if (parent != null)
            {
                BaseDirectoryEntry objparent = parent as BaseDirectoryEntry;
                objparent.AddChild(newObj);
            }

            return newObj;
        }

        public virtual ICloudDirectoryEntry CreateDirectoryEntry(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            // build up query url
            BaseDirectoryEntry newObj = new BaseDirectoryEntry(Name, 0, DateTime.Now, this, session);

            // case the parent if possible
            if (parent != null)
            {
                BaseDirectoryEntry objparent = parent as BaseDirectoryEntry;
                objparent.AddChild(newObj);
            }

            return newObj;
        }
    }
}
