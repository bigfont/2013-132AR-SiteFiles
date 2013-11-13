using System;
using System.IO;

using AppLimit.CloudComputing.SharpBox.DropBox;
using AppLimit.CloudComputing.SharpBox.Common;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox
{
    public delegate void FileOperationProgressChanged(ICloudFileSystemEntry file,  long currentbytes, long sizebytes);
 
    public partial class CloudStorage
    {
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
            var ph = new PathHelper(path);
            if (!ph.IsPathRooted())
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);

            return GetFolder(path, null);
        }

        public ICloudDirectoryEntry GetFolder(String path, ICloudDirectoryEntry parent)
        {
            var dir = GetFileSystemObject(path, parent) as ICloudDirectoryEntry;
            if (dir == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);
        	
			return dir;
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
            ICloudFileSystemEntry fsEntry = GetFileSystemObject(path, startFolder);
            if (fsEntry is ICloudDirectoryEntry)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        	
			return fsEntry;
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
            DownloadFile(parent, name, targetPath, null);
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
        /// <param name="delProgress"></param>
        /// <returns></returns>
        public void DownloadFile(ICloudDirectoryEntry parent, String name, String targetPath, FileOperationProgressChanged delProgress)
        {
            // check parameters
            if (parent == null || name == null || targetPath == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);


#if !WINDOWS_PHONE && !ANDROID
            // expand environment in target path
            targetPath = Environment.ExpandEnvironmentVariables(targetPath);
#endif
            // get the file entry
			ICloudFileSystemEntry file = parent.GetChild(name);

			// download the data
			using (Stream srcdata = file.GetContentStream(FileAccess.Read))
			{
				// build a filestream to target
				using (var targetData = new FileStream(Path.Combine(targetPath, name), FileMode.Create, FileAccess.Write, FileShare.None))
				{                    
					// copy the stream
                    StreamHelper.CopyStreamData(srcdata, targetData, FileStreamCopyCallback, delProgress, file );
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
            DownloadFile(filePath, targetPath, null);
        }

        /// <summary>
        /// This function allows to download a specific file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="delProgress"></param>
        /// <returns></returns>        
        public void DownloadFile(String filePath, String targetPath, FileOperationProgressChanged delProgress)
        {
            // check parameter
            if (filePath == null || targetPath == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // get path and filename
            var ph = new PathHelper(filePath);
            String dir = ph.GetDirectoryName();
            String file = ph.GetFileName();

            // check if we are in root
            if (dir.Length == 0)
                dir = "/";

            // get parent container
            ICloudDirectoryEntry container = GetFolder(dir);

            // download file
            DownloadFile(container, file, targetPath, delProgress);
        }

        /// <summary>
		/// This function allowes to upload a local file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetContainer"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(String filePath, ICloudDirectoryEntry targetContainer)
        {
            return UploadFile(filePath, targetContainer, (FileOperationProgressChanged)null);
        }

		/// <summary>
		/// This function allowes to upload a local file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetContainer"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(String filePath, ICloudDirectoryEntry targetContainer, FileOperationProgressChanged delProgress)
		{
			// check parameters
			if (String.IsNullOrEmpty(filePath))
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            return UploadFile(filePath, targetContainer, Path.GetFileName(filePath), delProgress);
		}

        /// <summary>
		/// This function allowes to upload a local file. Remote file will be created with the name specifed by
		/// the targetFileName argument
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetContainer"></param>
		/// <param name="targetFileName"></param>        
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(String filePath, ICloudDirectoryEntry targetContainer, string targetFileName)
        {
            return UploadFile(filePath, targetContainer, targetFileName, null);
        }

		/// <summary>
		/// This function allowes to upload a local file. Remote file will be created with the name specifed by
		/// the targetFileName argument
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetContainer"></param>
		/// <param name="targetFileName"></param>
        /// <param name="delProgress"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(String filePath, ICloudDirectoryEntry targetContainer, string targetFileName, FileOperationProgressChanged delProgress)
		{
			// check parameter
			if (String.IsNullOrEmpty(filePath) || String.IsNullOrEmpty(targetFileName) || targetContainer == null)
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

			// check if the target is a real file
			if (!File.Exists(filePath))
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);

			// build the source stream
			using (var srcStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// create the upload file
				ICloudFileSystemEntry newFile = CreateFile(targetContainer, targetFileName);
				if (newFile == null)
					throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);

				// content stream
				using (Stream dataStream = newFile.GetContentStream(FileAccess.Write))
				{
					// copy the data
                    StreamHelper.CopyStreamData(srcStream, dataStream, FileStreamCopyCallback, delProgress, newFile);
				}

				return newFile;
			}
		}

        /// <summary>
		/// This function allows to upload a local file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetDirectory"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(string filePath, string targetDirectory)
        {
            return UploadFile(filePath, targetDirectory, (FileOperationProgressChanged)null);
        }

		/// <summary>
		/// This function allows to upload a local file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetDirectory"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(string filePath, string targetDirectory, FileOperationProgressChanged delProgress)
		{
			// check parameters
			if (String.IsNullOrEmpty(filePath))
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            return UploadFile(filePath, targetDirectory, Path.GetFileName(filePath), delProgress);
		}

        		/// <summary>
		/// This function allows to upload a local file. Remote file will be created with the name specifed by
		/// the targetFileName argument
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetDirectory"></param>
		/// <param name="targetFileName"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(string filePath, string targetDirectory, string targetFileName)
        {
            return UploadFile(filePath, targetDirectory, targetFileName, null);
        }

		/// <summary>
		/// This function allows to upload a local file. Remote file will be created with the name specifed by
		/// the targetFileName argument
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="targetDirectory"></param>
		/// <param name="targetFileName"></param>
		/// <returns></returns>
        public ICloudFileSystemEntry UploadFile(string filePath, string targetDirectory, string targetFileName, FileOperationProgressChanged delProgress)
		{
			// check parameters
			if (String.IsNullOrEmpty(filePath) || String.IsNullOrEmpty(targetFileName) || targetDirectory == null)
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

			// get target container
			ICloudDirectoryEntry target = GetFolder(targetDirectory);
			if (target == null)
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);

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
            var ph = new PathHelper(path);

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
            var ph = new PathHelper(path);

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

        /// <summary>
        /// This function allows to delete a specific file or directory
        /// </summary>
        /// <param name="filePath">File or directory path which has to be deleted</param>
        /// <returns></returns>        
        public bool DeleteFileSystemEntry(String filePath)
        {
            // check parameter
            if (filePath == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // get path and filename
            var ph = new PathHelper(filePath);
            String dir = ph.GetDirectoryName();
            String file = ph.GetFileName();

            // check if we are in root
            if (dir.Length == 0)
                dir = "/";

            // get parent container
            ICloudDirectoryEntry container = GetFolder(dir);

            // get filesystem entry
            ICloudFileSystemEntry fsEntry = GetFileSystemObject(file, container);

            // delete file
            return DeleteFileSystemEntry(fsEntry);
        }

        /// <summary>
        /// This method moves a file or a directory from its current
        /// location to a new onw
        /// </summary>
        /// <param name="filePath">Filesystem object which has to be moved</param>
        /// /// <param name="newParentPath">The new location of the targeted filesystem object</param>
        /// <returns></returns>        
        public bool MoveFileSystemEntry(String filePath, String newParentPath)
        {
            // check parameter
            if ((filePath == null) || (newParentPath == null))
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // get path and filename
            var ph = new PathHelper(filePath);
            String dir = ph.GetDirectoryName();
            String file = ph.GetFileName();

            // check if we are in root
            if (dir.Length == 0)
                dir = "/";

            // get parent container
            ICloudDirectoryEntry container = GetFolder(dir);

            // get filesystem entry
            ICloudFileSystemEntry fsEntry = GetFileSystemObject(file, container);

            // get new parent path
            ICloudDirectoryEntry newParent = GetFolder(newParentPath);

            // move file
            return MoveFileSystemEntry(fsEntry, newParent);
        }

        /// <summary>
        /// This mehtod allows to perform a server side renam operation which is basicly the same
        /// then a move operation in the same directory
        /// </summary>
        /// <param name="filePath">File or directory which has to be renamed</param>
        /// <param name="newName">The new name of the targeted filesystem object</param>
        /// <returns></returns>        
        public bool RenameFileSystemEntry(String filePath, String newName)
        {
            // check parameter
            if ((filePath == null) || (newName == null))
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // get path and filename
            var ph = new PathHelper(filePath);
            String dir = ph.GetDirectoryName();
            String file = ph.GetFileName();

            // check if we are in root
            if (dir.Length == 0)
                dir = "/";

            // get parent container
            ICloudDirectoryEntry container = GetFolder(dir);

            // get filesystem entry
            ICloudFileSystemEntry fsEntry = GetFileSystemObject(file, container);

            // rename file
            return RenameFileSystemEntry(fsEntry, newName);
        }

        /// <summary>
        /// This method creates a new file object in the cloud storage. Use the GetContentStream method to 
        /// get a .net stream which usable in the same way then local stream are usable
        /// </summary>
		/// <param name="filePath">The name of the targeted file</param>
        /// <returns></returns>        
        public ICloudFileSystemEntry CreateFile(String filePath)
        {
            // check parameter
            if (filePath == null)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            // get path and filename
            var ph = new PathHelper(filePath);
            String dir = ph.GetDirectoryName();
            String file = ph.GetFileName();

            // check if we are in root
            if (dir.Length == 0)
                dir = "/";

            // get parent container
            ICloudDirectoryEntry container = GetFolder(dir);

            // rename file
            return CreateFile(container, file);
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Just a helper for the stream copy process
        /// </summary>
        /// <param name="src"></param>
        /// <param name="trg"></param>
        /// <param name="BufferSize"></param>
        /// <param name="data"></param>
        private void FileStreamCopyCallback(long ReadByteTotal, long TotalLength, params Object[] data)
        {
            // check array
            if (data.Length != 2)
                return;

            // get the progess delegate
            FileOperationProgressChanged pc = data[0] as FileOperationProgressChanged;
            if (pc == null)
                return;

            // get the file
            ICloudFileSystemEntry e = data[1] as ICloudFileSystemEntry;
            if (e == null)
                return;

            if (TotalLength == -1)
                pc(e, ReadByteTotal, e.Length);
            else
                pc(e, ReadByteTotal, TotalLength);
        }

        #endregion
    }
}
