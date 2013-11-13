using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

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
    public class CloudStorage : ICloudStorageProvider
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
			if ( _configurationProviderMap.ContainsKey(configurationType))
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
        public ICloudStorageAccessToken Open(ICloudStorageConfiguration configuration, ICloudeStorageCredentials credentials)
        {
            // check state
            if (IsOpened)
                return null;

            // read out the right provider type
			Type providerType = null;
            if (!_configurationProviderMap.TryGetValue(configuration.GetType(), out providerType))
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorNoValidProviderFound);
							
			// build up the provider
            _provider = CreateProviderByType(providerType);            
			
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
        /// the cloude storage
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

        #endregion

        #region Comfort Functions

        /// <summary>
        /// This method returns access ro a specific subfolder addressed via 
        /// unix like file system path, e.g. /Public/SubFolder/SubSub
        /// 
        /// Valid Exceptions are:
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorFileNotFound);
        /// </summary>
        /// <param name="path">The path to the target subfolder</param>
        /// <returns></returns>
        public ICloudDirectoryEntry GetFolder(String path)
        {
            PathHelper ph = new PathHelper(path);
            if (!ph.IsPathRooted())
                return null;

            return GetFolder(path, null);
        }

        /// <summary>
        /// This method returns a folder without thrown an exception in the case of an error
        /// </summary>
        /// <param name="path"></param>
		/// <param name="throwException"></param>
        /// <returns></returns>
        public ICloudDirectoryEntry GetFolder(String path, Boolean throwException)
        {
			try
			{
				return GetFolder(path);
			}
			catch (SharpBoxException)
			{
				if (throwException)
					throw;

				return null;
			}
        }

        /// <summary>
        /// This method returns access ro a specific subfolder addressed via 
        /// unix like file system path, e.g. /Public/SubFolder/SubSub
        /// 
        /// Valid Exceptions are:
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorFileNotFound);
        /// </summary>
        /// <param name="path">The path to the target subfolder</param>
        /// <param name="startFolder">The startfolder for the searchpath</param>
        /// <returns></returns>
        public ICloudDirectoryEntry GetFolder(String path, ICloudDirectoryEntry startFolder)
        {
			var fsEntry = GetFileOrFolder(path, startFolder) as ICloudDirectoryEntry;
            if (fsEntry != null)
                return fsEntry;

        	throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        }

        /// <summary>
        /// This method ...
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startFolder"></param>
		/// <param name="throwException"></param>
        /// <returns></returns>
        public ICloudDirectoryEntry GetFolder(String path, ICloudDirectoryEntry startFolder, Boolean throwException)
        {
			try
			{
				return GetFolder(path, startFolder);
			}
			catch (SharpBoxException)
			{
				if (throwException)
					throw;

				return null;
			}
		}

        /// <summary>
        /// This method returns access to a specific file addressed via 
        /// unix like file system path, e.g. /Public/SubFolder/SubSub
        /// 
        /// Valid Exceptions are:
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorFileNotFound);
        /// </summary>
        /// <param name="path">The path to the target subfolder</param>
        /// <param name="startFolder">The startfolder for the searchpath</param>
        /// <returns></returns>
        public ICloudFileSystemEntry GetFile(String path, ICloudDirectoryEntry startFolder)
        {
            ICloudFileSystemEntry fsEntry = GetFileOrFolder(path, startFolder);
            if (fsEntry is ICloudDirectoryEntry)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
            else
                return fsEntry;
        }        

        /// <summary>
        /// This is the helper function for get file or folder
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startFolder"></param>
        /// <returns></returns>
        private ICloudFileSystemEntry GetFileOrFolder(String path, ICloudDirectoryEntry startFolder)
        {
            // parse the path
            PathHelper ph = new PathHelper(path);

            // check if we accept rooted path
            if (startFolder != null && ph.IsPathRooted())
                return null;

            // set the start folder
            if (startFolder == null)
                startFolder = GetRoot();

            // split the path into the elements
            String[] elements = ph.GetPathElements();

            // start to find the right folder
            ICloudFileSystemEntry entry = startFolder;

            foreach (String element in elements)
            {
                // convert to directory
                ICloudDirectoryEntry dir = entry as ICloudDirectoryEntry;
                if (dir == null)
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);                    

                // check the child item
                entry = dir.GetChild(element);
                if (entry == null)
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);
            }

            return entry;
        }

        /// <summary>
        /// This functions allows to download a specific file
        ///         
        /// Valid Exceptions are:        
        ///  SharpBoxException(nSharpBoxErrorCodes.ErrorFileNotFound);
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public void DownloadFile(ICloudDirectoryEntry parent, String name, String targetPath)
        {
            // check parameters
            if (parent == null || name == null || targetPath == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // expand environment in target path
            targetPath = Environment.ExpandEnvironmentVariables(targetPath);

            // build a filestream to target
            using(FileStream targetData = new FileStream(Path.Combine(targetPath, name), FileMode.Create, FileAccess.Write, FileShare.None))
            {                
                // get the file entry
                ICloudFileSystemEntry file = parent.GetChild(name);
                
                // download the data
                using (Stream srcdata = file.GetContentStream(FileAccess.Read))
                {                    
                    // copy the stream
                    StreamHelper.CopyStreamData(srcdata, targetData);
                }
            }            
        }

        /// <summary>
        /// This function allows to download a specific file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>        
        public void DownloadFile(String filePath, String targetPath)
        {
            // check parameter
            if (filePath == null || targetPath == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // get path and filename
            PathHelper ph = new PathHelper(filePath);
            String dir = ph.GetDirectoryName();
            String file = ph.GetFileName();

            // check if we are in root
            if (dir.Length == 0)
                dir = "/";

            // get parent container
            ICloudDirectoryEntry container = this.GetFolder(dir);
            
            // download file
            DownloadFile(container, file, targetPath);
        }

        /// <summary>
        /// This function allowes to upload a local file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetContainer"></param>
        /// <returns></returns>
        public Boolean UploadFile(String filePath, ICloudDirectoryEntry targetContainer)
        {
            // check parameter
            if (filePath == null || targetContainer == null)
                return false;

            // check if the target is a real file
            if ( !File.Exists(filePath) )
                return false;

            // get filename
            PathHelper ph = new PathHelper(filePath);
            String file = ph.GetFileName();

            // build the source stream
            using (FileStream srcStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // create the upload file
                ICloudFileSystemEntry newFile = CreateFile(targetContainer, file);
                if (newFile == null)
                    return false;

                // content stream
                using (Stream dataStream = newFile.GetContentStream(FileAccess.Write))
                {
                    // copy the data
                    StreamHelper.CopyStreamData(srcStream, dataStream);
                }
            }

            return true;
        }

        /// <summary>
        /// This function allows to upload a local file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public Boolean UploadFile(String filePath, String targetDirectory)
        {
            // check parameters
            if (filePath == null || targetDirectory == null)
                return false;

            // get target container
            ICloudDirectoryEntry target = GetFolder(targetDirectory);
            if (target == null)
                return false;

            // upload file
            return UploadFile(filePath, target);
        }

        /// <summary>
        /// This function allows to create full folder pathes in the cloud storage
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICloudDirectoryEntry CreateFolder(String path)
        {
            // get the path elemtens
            PathHelper ph = new PathHelper(path);

            // check parameter
            if (!ph.IsPathRooted())
                return null;

            // start creation
            return CreateFolderEx(path, GetRoot());
        }

        /// <summary>
        /// This function allows to create full folder pathes in the cloud storage
        /// </summary>
        /// <param name="path"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public ICloudDirectoryEntry CreateFolderEx(String path, ICloudDirectoryEntry entry)
        {
            // get the path elemtens
            PathHelper ph = new PathHelper(path);
            
            String[] pes = ph.GetPathElements();
            
            // check which elements are existing            
            foreach (String el in pes)
            {
				// check if subfolder exists, if it doesn't, create it
                ICloudDirectoryEntry cur = GetFolder(el, entry, false) ?? CreateFolder(el, entry);

            	// go ahead
                entry = cur;                
            }
            
            // return 
            return entry;
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

        #endregion
    }
}
