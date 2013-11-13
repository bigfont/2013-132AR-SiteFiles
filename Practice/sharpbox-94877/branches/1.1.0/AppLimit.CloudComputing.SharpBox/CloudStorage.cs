using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

#if !WINDOWS_PHONE
using System.Net.Security;
#endif

using AppLimit.CloudComputing.SharpBox.Common;
using AppLimit.CloudComputing.SharpBox.Exceptions;

using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
#if !WINDOWS_PHONE
using AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav;
using System.Net;
using AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS;
#endif



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
        private ICloudStorageConfiguration _configuration;
        private readonly Dictionary<Type, Type> _configurationProviderMap;

        #endregion

        #region Constructure and logistics

        /// <summary>
        /// returns the currently setted access token 
        /// </summary>
        public ICloudStorageAccessToken CurrentAccessToken 
        {
            get
            {
                if (_provider == null)
                    return null;
                else
                    return _provider.CurrentAccessToken;
            }
        }

        /// <summary>
        /// Allows access to the current configuration which was used in the open call
        /// </summary>
        public ICloudStorageConfiguration CurrentConfiguration
        {
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// The default constructure for a cloudstorage 
        /// </summary>
        public CloudStorage()
        {
            // build config provider
            _configurationProviderMap = new Dictionary<Type, Type>();

            // register provider
            RegisterStorageProvider(typeof(DropBoxConfiguration), typeof(DropBoxStorageProvider));			
#if !WINDOWS_PHONE
            RegisterStorageProvider(typeof(WebDavConfiguration), typeof(WebDavStorageProvider));
            RegisterStorageProvider(typeof(CIFSConfiguration), typeof(CIFSStorageProvider));
#endif
        }

        /// <summary>
        /// copy ctor 
        /// </summary>
        /// <param name="src"></param>
        public CloudStorage(CloudStorage src)
            : this()
        {
            if (src.IsOpened)
                Open(src._configuration, src.CurrentAccessToken);
            else
                _configuration = src._configuration;
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

#if !WINDOWS_PHONE
        /// <summary>
        /// Ignores all invalid ssl certs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        static bool ValidateAllServerCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
#endif

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

            // save the configuration
            _configuration = configuration;

#if !WINDOWS_PHONE
            // verify the ssl config
            if (configuration.TrustUnsecureSSLConnections)
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateAllServerCertificates;
            
            // update the max connection settings 
            ServicePointManager.DefaultConnectionLimit = 250;

            // disable the not well implementes Expected100 header settings
            ServicePointManager.Expect100Continue = false;
#endif

            // open the cloud connection
            ICloudStorageAccessToken token = _provider.Open(configuration, credentials);

            // ok without Exception every is good
            IsOpened = true;

            // return the token
            return token;
        }

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
        /// <param name="token">Vendor specific authorization token</param>        
        /// <returns>A valid access token or null</returns>
        public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudStorageAccessToken token)
        {
            // check state
            if (IsOpened)
                return null;

            // ensures that the right provider will be used
            SetProviderByConfiguration(configuration);
            
            // save the configuration
            _configuration = configuration;

            // open the cloud connection                                    
            token = _provider.Open(configuration, token);

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
        /// <param name="name">The path to the target subfolder</param>
        /// <param name="parent">The startfolder for the searchpath</param>
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

        /// <summary>
        /// Returns the path of the targeted object
        /// </summary>
        /// <param name="fsObject"></param>
        /// <returns></returns>
        public String GetFileSystemObjectPath(ICloudDirectoryEntry fsObject)
        {
            // pass through the provider
            return _provider == null ? null : _provider.GetFileSystemObjectPath(fsObject);
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
            
            // check the type
            Object obj = serializer.ReadObject(tokenStream);
            if ( !obj.GetType().Equals(typeof(List<string>)))
#if SILVERLIGHT || ANDROID || MONOTOUCH
                throw new Exception("A List<String> was expected");
#else
                throw new InvalidDataException("A List<String> was expected");
#endif

            // build the token
            return LoadToken((List<string>)obj);
        }

        /// <summary>
        /// This method stores the content of an access token in to 
        /// a list of string. This list can be serialized.
        /// </summary>
        /// <param name="tokendata">Target list</param>
        /// <param name="token">the token</param>
        public void StoreToken(List<string> tokendata, ICloudStorageAccessToken token)
        {
            _provider.StoreToken(tokendata, token);
        }

        /// <summary>
        /// This method generated a access token from the given data 
        /// string list
        /// </summary>
        /// <param name="tokendata">the string list</param>
        /// <returns>The unserialized token</returns>
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
