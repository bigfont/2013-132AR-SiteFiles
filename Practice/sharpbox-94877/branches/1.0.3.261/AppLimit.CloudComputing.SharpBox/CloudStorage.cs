using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using AppLimit.CloudComputing.SharpBox.DropBox;
using AppLimit.CloudComputing.SharpBox.Common;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// The class CloudStorage implements all needed methods to get access
    /// to a supported cloud storage infrastructure. The following vendors
    /// are currently supported:
    /// 
    ///  - DropBox
    ///  
    /// </summary>
    public partial class CloudStorage : ICloudStorageProvider, ICloudStorageAsyncInterface
    {
        #region Member Declarations

        private ICloudStorageProvider _provider;
        private readonly Dictionary<Type, Type> _configurationProviderMap;

        #endregion

        #region Constructure and logistics

        /// <summary>
        /// The default constructure for a cloudstorage 
        /// </summary>
        public CloudStorage()
        {
            // build config provider
            _configurationProviderMap = new Dictionary<Type, Type>();

            // register provider
            RegisterStorageProvider(typeof(DropBoxConfiguration), typeof(DropBoxStorageProvider));
        }

        /// <summary>
        /// This method allows to register a storage provider for a specific configuration
        /// type
        /// </summary>
        /// <param name="configurationType">
        /// A <see cref="Type"/>
        /// </param>
        /// <param name="storageProviderType">
        /// A <see cref="Type"/>
        /// </param>
        /// <returns>
        /// A <see cref="Boolean"/>
        /// </returns>
        public Boolean RegisterStorageProvider(Type configurationType, Type storageProviderType)
        {
            // do double check
            if (_configurationProviderMap.ContainsKey(configurationType))
                return false;

            // register
            _configurationProviderMap.Add(configurationType, storageProviderType);

            // go ahead
            return true;
        }

        #endregion

        #region Base Functions

        /// <summary>
        /// True when a the connection to the Cloudstorage will be established
        /// </summary>
        public Boolean IsOpened { get; private set; }

        /// <summary>
        /// Calling this method with vendor specific configuration settings and credentials
        /// to get access to the cloud storage. The following exceptions are possible:
        /// 
        /// - System.UnauthorizedAccessException when the user provides wrong credentials
        /// - AppLimit.CloudComputing.SharpBox.Exceptions.SharpBoxException when something 
        ///   happens during cloud communication
        /// 
        /// </summary>
        /// <param name="configuration">Vendor specific configuration of the cloud storage</param>
        /// <param name="credentials">Vendor specific credentials for authorization</param>        
        /// <returns>A valid access token or null</returns>
        public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudStorageCredentials credentials)
        {
            // check state
            if (IsOpened)
                return null;

            // ensures that the right provider will be used
            SetProviderByConfiguration(configuration);

            // open the cloud connection
            ICloudStorageAccessToken token = _provider.Open(configuration, credentials);

            // ok without Exception every is good
            IsOpened = true;

            // return the token
            return token;
        }        

        /// <summary>
        /// This method will close the connection to the cloud storage system
        /// </summary>
        public void Close()
        {
            if (_provider == null)
                return;

            _provider.Close();

            IsOpened = false;
        }

        /// <summary>
        /// This method returns access to the root folder of the storage system
        /// </summary>
        /// <returns>Reference to the root folder of the storage system</returns>
        public ICloudDirectoryEntry GetRoot()
        {
            return _provider == null ? null : _provider.GetRoot();
        }

        /// <summary>
        /// This method returns access ro a specific subfolder or file addressed via 
        /// unix like file system path, e.g. /Public/SubFolder/SubSub
        /// 
        /// Valid Exceptions are:
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorFileNotFound);
        /// </summary>
        /// <param name="path">The path to the target subfolder</param>
        /// <param name="startFolder">The startfolder for the searchpath</param>
        /// <returns></returns>
        public ICloudFileSystemEntry GetFileSystemObject(String name, ICloudDirectoryEntry parent)
        {
            return _provider == null ? null : _provider.GetFileSystemObject(name, parent);
        }

        /// <summary>
        /// This method creates a child folder in the give folder
        /// of the cloud storage
        /// </summary>
        /// <param name="name">Name of the new folder</param>
        /// <param name="parent">Parent folder</param>
        /// <returns>Reference to the created folder</returns>
        public ICloudDirectoryEntry CreateFolder(String name, ICloudDirectoryEntry parent)
        {
            return _provider == null ? null : _provider.CreateFolder(name, parent);
        }

        /// <summary>
        /// This method removes a file or a directory from 
        /// the cloud storage
        /// </summary>
        /// <param name="fsentry">Reference to the filesystem object which has to be removed</param>
        /// <returns>Returns true or false</returns>
        public bool DeleteFileSystemEntry(ICloudFileSystemEntry fsentry)
        {
            return _provider == null ? false : _provider.DeleteFileSystemEntry(fsentry);
        }

        /// <summary>
        /// This method moves a file or a directory from its current
        /// location to a new onw
        /// </summary>
        /// <param name="fsentry">Filesystem object which has to be moved</param>
        /// <param name="newParent">The new location of the targeted filesystem object</param>
        /// <returns></returns>
        public bool MoveFileSystemEntry(ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            return _provider == null ? false : _provider.MoveFileSystemEntry(fsentry, newParent);
        }

        /// <summary>
        /// This mehtod allows to perform a server side renam operation which is basicly the same
        /// then a move operation in the same directory
        /// </summary>
        /// <param name="fsentry"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool RenameFileSystemEntry(ICloudFileSystemEntry fsentry, String newName)
        {
            return _provider == null ? false : _provider.RenameFileSystemEntry(fsentry, newName);
        }

        /// <summary> 
        /// This method creates a new file object in the cloud storage. Use the GetContentStream method to 
        /// get a .net stream which usable in the same way then local stream are usable.
        /// </summary>
        /// <param name="parent">Link to the parent container, null means the root directory</param>
        /// <param name="Name">The name of the targeted file</param>        
        /// <returns></returns>
        public ICloudFileSystemEntry CreateFile(ICloudDirectoryEntry parent, String Name)
        {
            // pass through the provider
            return _provider == null ? null : _provider.CreateFile(parent, Name);
        }
        /// <summary>
        /// This method returns the direct URL to a specific file system object,
        /// e.g. a file or folder
        /// </summary>
        /// <param name="path">A relative path to the file</param>
        /// <param name="parent">A reference to the parent of the path</param>
        /// <returns></returns>
        public Uri GetFileSystemObjectUrl(String path, ICloudDirectoryEntry parent)
        {
            // pass through the provider
            return _provider == null ? null : _provider.GetFileSystemObjectUrl(path, parent);
        }        

        #endregion

        #region AccessTokenHandling

        /// <summary>
        /// This method allows to store a security token into a serialization stream
        /// </summary>        
        /// <param name="token"></param>
        /// <returns></returns>
        public Stream SerializeSecurityToken(ICloudStorageAccessToken token)
        {            
            // prepares the serializer
            string type = token.GetType().ToString();

            var items = new List<string>();
            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(items.GetType());

            items.Add(type);

            // save the token into our list
            StoreToken(items, token);            

            // write the data to stream
            serializer.WriteObject(stream, items);
            
            // go to start
            stream.Seek(0, SeekOrigin.Begin);

            // go ahead
            return stream;
        }

        /// <summary>
        /// This method allows to load a token from a previously generated stream
        /// </summary>
        /// <param name="tokenStream"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ICloudStorageAccessToken DeserializeSecurityToken(Stream tokenStream, ICloudStorageConfiguration configuration)
        {
            // ensures that the right provider will be used
            SetProviderByConfiguration(configuration);

            // load the data in our list            
            var serializer = new DataContractSerializer(typeof(List<string>));
            List<string> items = serializer.ReadObject(tokenStream) as List<string>;
                       
            // build the token
            return LoadToken(items);
        }

        public void StoreToken(List<string> tokendata, ICloudStorageAccessToken token)
        {
            _provider.StoreToken(tokendata, token);
        }

        public ICloudStorageAccessToken LoadToken(List<string> tokendata)
        {
            return _provider.LoadToken(tokendata);
        }

        #endregion

        #region Helper

        private static ICloudStorageProvider CreateProviderByType(Type providerType)
        {
            ICloudStorageProvider provider;

            try
            {
                provider = Activator.CreateInstance(providerType) as ICloudStorageProvider;
            }
            catch (Exception e)
            {
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCreateProviderInstanceFailed, e);
            }

            if (provider == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCreateProviderInstanceFailed);

            return provider;
        }

        private void SetProviderByConfiguration(ICloudStorageConfiguration configuration)
        {
            // check
            if (configuration == null && _provider == null)
                throw new InvalidOperationException("It's only allowed to set the configuration parameter to null when a provider was set before");

            // read out the right provider type
            Type providerType = null;
            if (!_configurationProviderMap.TryGetValue(configuration.GetType(), out providerType))
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorNoValidProviderFound);

            // build up the provider
            _provider = CreateProviderByType(providerType);
        }

        #endregion      
    }
}
