// ----------------------------------------------------------------------------------------
// FTP Provider Functions - by Fungusware [www.fungusware.com]
// ----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

using AppLimit.CloudComputing.SharpBox.Common.IO;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.FTP.Logic
{
    internal class FTPStorageProviderService : GenericStorageProviderService
    {
        private FTPclient _context;
        private Dictionary<String, ICloudDirectoryEntry> _validatedFolders;

        public override bool VerifyCredentialType(ICloudStorageCredentials credentials)
        {
            return (credentials is ICredentials);
        }

        public override IStorageProviderSession CreateSession(ICloudStorageCredentials credentials, ICloudStorageConfiguration configuration)
        {            
            // cast the creds to the right type
            GenericNetworkCredentials creds = credentials as GenericNetworkCredentials ;
            FTPConfiguration config = configuration as FTPConfiguration;

            _context = new FTPclient(config.ServiceLocator.ToString(), creds.UserName, creds.Password);
            _validatedFolders = new Dictionary<String, ICloudDirectoryEntry>();

            try
            {
                string sRoot = _context.CurrentDirectory;
				if (sRoot == String.Empty)
					return null;
            } 
            catch(Exception)
            {
                return null;
            }
            
            return new FTPStorageProviderSession(credentials as ICloudStorageAccessToken, configuration as FTPConfiguration, this);            
        }
                
        public override ICloudFileSystemEntry RequestResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            // build path
            String path;
            
            if (Name.Equals("/"))
                path = session.ServiceConfiguration.ServiceLocator.LocalPath;
            else if (parent == null)
                path = Path.Combine(path = session.ServiceConfiguration.ServiceLocator.LocalPath, Name);
            else
                path = new Uri(GetResourceUrl(session, parent, Name)).LocalPath;

            // check if directory exists
            if (_validatedFolders.ContainsKey(path))
                return _validatedFolders[path];

            else
            {
                if (_context.FtpDirectoryExists(path))
                {
                    ICloudDirectoryEntry bd;

                    if (Name.Equals("/"))
                    {
                        bd = GenericStorageProviderFactory.CreateDirectoryEntry(session, "/", null);                        
                    }
                    else
                    {
                        // build directory info -- we want the parent here
                        String uriPath = GetResourceUrl(session, parent, null);
                        Uri tgUri = new Uri(uriPath);
                        FTPdirectory parentdir = _context.ListDirectoryDetail(tgUri.LocalPath);
                        FTPfileInfo dir = parentdir.Find(delegate(FTPfileInfo f)
                        {
                            return f.Filename == System.IO.Path.GetFileName(path);
                        });

                        // build  dir
                        bd = GenericStorageProviderFactory.CreateDirectoryEntry(session, dir.Filename, dir.FileDateTime.Value, parent);                        
                    };
                    
                    // refresh the childs
                    RefreshChildsOfDirectory(session, bd);

                    // store for later
                    _validatedFolders.Add(path, bd);

                    // go ahead
                    return bd;
                }
                // check if file exists
                else if (_context.FtpFileExists(path))
                {
                    // create the fileinfo
                    String uriPath = GetResourceUrl(session, parent, null);
                    Uri tgUri = new Uri(uriPath);
                    FTPdirectory dir = _context.ListDirectoryDetail(tgUri.LocalPath);
                    FTPfileInfo file = dir.Find(delegate(FTPfileInfo f)
                    {
                        return f.Filename == System.IO.Path.GetFileName(path);
                    });

                    return GenericStorageProviderFactory.CreateFileSystemEntry(session, file.Filename, file.FileDateTime.Value, file.Size, parent);                    
                }
                else
                    return null;
            }
        }

        public override void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource)
        {
            // nothing to do for files
            if (!(resource is ICloudDirectoryEntry))
                return;

            // Refresh schild
            RefreshChildsOfDirectory(session, resource as ICloudDirectoryEntry);
        }

        public override bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            // generate the loca path
            String uriPath = GetResourceUrl(session, entry, null);
            Uri uri = new Uri(uriPath);

            // removed the file
            if (_context.FtpFileExists(uri.LocalPath))
                _context.FtpDelete(uri.LocalPath);
            else if (_context.FtpDirectoryExists(uri.LocalPath))
                _context.FtpDeleteDirectory(uri.LocalPath);
            
            // go ahead
            return true;
                
        }

        public override bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            // build the new uri
            //String newPlace = GetResourceUrl(session, newParent, fsentry.Name);

            //if (RenameResourceEx(session, fsentry, newPlace))
            //{
            //    // remove from parent
            //    (fsentry.Parent as BaseDirectoryEntry).RemoveChild(fsentry as BaseFileEntry);

            //    // add to new parent
            //    (newParent as BaseDirectoryEntry).AddChild(fsentry as BaseFileEntry);

            //    return true;
            //}
            //else
                return false;
        }
              
        public override Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry)
        {
            // get the full path
            String uriPath = GetResourceUrl(session, fileSystemEntry, null);

            System.Net.FtpWebRequest ftp = _context.GetRequest(uriPath);

            //Set request to download a file in binary mode
            ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            ftp.UseBinary = true;

            //open request and get response stream
            FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

            // open src file
            return response.GetResponseStream();
        }

        public override Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, long uploadSize)
        {
            // get the full path
            String uriPath = GetResourceUrl(session, fileSystemEntry, null);
            System.Net.FtpWebRequest ftp = _context.GetRequest(uriPath);

            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
            ftp.UseBinary = true;

            //Notify FTP of the expected size
            ftp.ContentLength = uploadSize;            

            // go ahead
            return ftp.GetRequestStream();
        }

        public override void CommitStreamOperation(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, nTransferDirection Direction, Stream NotDisposedStream)
        {

        }

        public override ICloudFileSystemEntry CreateResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            // build the full url
            String resFull = GetResourceUrl(session, parent, Name);
            Uri uri = new Uri(resFull);
            String sMsg = "";
            
            // create the director
           if (_context.FtpCreateDirectory(uri.LocalPath, ref sMsg))
           {
                // create the filesystem object
               ICloudDirectoryEntry fsEntry = GenericStorageProviderFactory.CreateDirectoryEntry(session, Name, parent);                                       

                // Store for later
                _validatedFolders.Add(uri.LocalPath, fsEntry);
                
                // go ahead
                return fsEntry;
            }
            else
                return null;
        }

        public override bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, string newName)
        {            
            // get new name uri
            String uriPath = GetResourceUrl(session, fsentry.Parent, newName);
            
            // do it 
            return _context.FtpRename(fsentry.Name, uriPath.ToString());
        }        


        #region Helper

        private void RefreshChildsOfDirectory(IStorageProviderSession session, ICloudDirectoryEntry dir)
        {
            // get dir info
            String uriPath = GetResourceUrl(session, dir, null);
            Uri tgUri = new Uri(uriPath);
            FTPdirectory parentdir = _context.ListDirectoryDetail(tgUri.LocalPath);

            // clear childs
            GenericStorageProviderFactory.ClearAllChilds(dir);            

            // get all childs
            foreach (FTPfileInfo  fInfo in parentdir)
            {
                if (fInfo.FileType == FTPfileInfo.DirectoryEntryTypes.Directory)
                {
                    GenericStorageProviderFactory.CreateDirectoryEntry( session, fInfo.Filename , fInfo.FileDateTime.Value, dir);                    
                }
                else
                {
                    GenericStorageProviderFactory.CreateFileSystemEntry(session, fInfo.Filename, fInfo.FileDateTime.Value, fInfo.Size, dir);                   
                }
            }
        }

        //public bool RenameResourceEx(IStorageProviderSession session, ICloudFileSystemEntry fsentry, String newFullPath)
        //{
        //    // get the uri
        //    String uriPath = GetResourceUrl(session, fsentry, null);
        //    Uri srUri = new Uri(uriPath);

        //    // get new name uri            
        //    Uri tgUri = new Uri(newFullPath);

        //    // rename
        //    FileSystemInfo f = null;
        //    if (File.Exists(srUri.LocalPath))
        //    {
        //        f = new FileInfo(srUri.LocalPath);
        //        ((FileInfo)f).MoveTo(tgUri.LocalPath);
        //    }
        //    else if (Directory.Exists(srUri.LocalPath))
        //    {
        //        f = new DirectoryInfo(srUri.LocalPath);
        //        ((DirectoryInfo)f).MoveTo(tgUri.LocalPath);
        //    }
        //    else
        //    {
        //        throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);
        //    }

        //    // reload file info 
        //    if (File.Exists(tgUri.LocalPath))
        //    {
        //        f = new FileInfo(tgUri.LocalPath);
        //    }
        //    else if (Directory.Exists(tgUri.LocalPath))
        //    {
        //        f = new DirectoryInfo(tgUri.LocalPath);
        //    }
        //    else
        //    {
        //        throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);
        //    }

        //    // update fsEntry
        //    BaseFileEntry fs = fsentry as BaseFileEntry;
        //    fs.Name = Path.GetFileName(tgUri.LocalPath);
        //    fs.Length = (f is FileInfo ? (f as FileInfo).Length : 0);
        //    fs.Modified = f.LastWriteTimeUtc;

        //    // go ahead
        //    return true;
        //}
        #endregion                  
    }
}
