using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common;

using AppLimit.CloudComputing.OAuth;
using AppLimit.CloudComputing.OAuth.Context;
using AppLimit.CloudComputing.OAuth.Token;
using AppLimit.CloudComputing.SharpBox.DropBox.Objects;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxApplication
    {
        private const String DropBoxCallbackUrl = "http://www.applimit.com";

        // server and base url 
		public const String DropBoxServer = "api.dropbox.com";
		public const String DropBoxContentServer = "api-content.dropbox.com";
		public const String DropBoxBaseUrl = "http://" + DropBoxServer;

        // api version
		public const String DropBoxApiVersion = "0";

		public const String DropBoxGetAccountInfo = "http://" + DropBoxServer + "/" + DropBoxApiVersion + "/account/info";
		public const String DropBoxSandboxRoot = "http://" + DropBoxServer + "/" + DropBoxApiVersion + "/metadata/sandbox/";
		public const String DropBoxDropBoxRoot = "http://" + DropBoxServer + "/" + DropBoxApiVersion + "/metadata/dropbox/";
		public const String DropBoxCreateFolder = "http://" + DropBoxServer + "/" + DropBoxApiVersion + "/fileops/create_folder";
		public const String DropBoxDeleteItem = "http://" + DropBoxServer + "/" + DropBoxApiVersion + "/fileops/delete";
		public const String DropBoxMoveItem = "http://" + DropBoxServer + "/" + DropBoxApiVersion + "/fileops/move";

		public const String DropBoxUploadDownloadFile = "http://" + DropBoxContentServer + "/" + DropBoxApiVersion + "/files";        

		public const String DropBoxMobileLogin = "https://" + DropBoxServer + "/" + DropBoxApiVersion + "/token";
		
        // private const String urlCreateFolder;
        private readonly DropBoxService _service;
        private readonly String _consumerKey;
        private readonly String _consumerSecret;        

        public DropBoxApplication(DropBoxService service, String consumerKey, String consumerSecret)
        {
            _service = service;

            // save the application information
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;            
        }

        public DropBoxSession Authorize(String userName, String password )
        {
            // Get a valid dropbox session through oAuth authorization
            DropBoxSession session = AuthorizeAndGetSession(userName, password);

            //( Get a valid root object
            ICloudDirectoryEntry root = GetRootBySession(session);
			
			// return the infos
			return root == null ? null : session;
        }

        public DropBoxSession Authorize(DropBoxToken token)
        {
            // Get a valid dropbox session through oAuth authorization
            DropBoxSession session = BuildSessionFromAccessToken(token);

            //( Get a valid root object
            ICloudDirectoryEntry root = GetRootBySession(session);

            // return the infos
            return root == null ? null : session;
        }

        private ICloudDirectoryEntry GetRootBySession(DropBoxSession session)
        {
            // now check if we have application keys and secrets from a sandbox or a fullbox
            // try to load the root of the full box if this fails we have only sandbox access
            ICloudDirectoryEntry root = GetRoot(session);
            if (root == null)
            {
                // disbale sandbox mode
                session.SandBoxMode = true;

                // retry to get root object
                root = GetRoot(session);
            }
            return root;
        }        

        private DropBoxSession AuthorizeAndGetSession(String userName, String password)
        {         
            // build the consumer context
            var consumerContext = new OAuthConsumerContext(_consumerKey, _consumerSecret);

            // build up the oauth session
            var serviceContext = new OAuthServiceContext(_service.Configuration.GetRequestTokenUrl().ToString(),
                                                            _service.Configuration.GetAuthorizationTokenUrl().ToString(),
                                                            DropBoxCallbackUrl,
                                                            _service.Configuration.GetAccessTokenUrl().ToString());

            // get a request token from the provider
            var requestToken = OAuthService.GetRequestToken(serviceContext, consumerContext);


            // build up a request Token Session
            var requestSession = new DropBoxSession(requestToken, consumerContext);

            // build up the parameters
            var param = new Dictionary<String, String>
            {
            	{ "email", userName }, 
				{ "password", password }
            };

        	// call the mobile login api 
            String result = "";

            try
            {
                result = requestSession.RequestRessourceByUrl(DropBoxMobileLogin, param);
                if (result.Length == 0)
                    throw new UnauthorizedAccessException();
            }
#if !MONOTOUCH
            catch (HttpException netex) 
            {
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotContactStorageService, netex);
            }
#else
			catch (Exception ex)
			{
				if (ex is UnauthorizedAccessException )
					throw ex;
				else
					throw new SharpBoxException(nSharpBoxErrorCodes.ErrorCouldNotContactStorageService, ex);
			}
#endif

            // exchange a request token for an access token
            var accessToken = new DropBoxToken(result);

            // build the session
            return BuildSessionFromAccessToken(accessToken);            
        }

        public DropBoxSession BuildSessionFromAccessToken(DropBoxToken token)
        {
            // build the consumer context
            var consumerContext = new OAuthConsumerContext(_consumerKey, _consumerSecret);
                        
            // build the session
            var session = new DropBoxSession(token, consumerContext);

            // go aahead
            return session;
        }

        public void Close(DropBoxSession session)
        {            
            
        }

        public DropBoxAccountInfo GetAccountInfo(DropBoxSession session)
        {
            // request the json object via oauth            
            var res = session.RequestRessourceByUrl(DropBoxGetAccountInfo);

            // parse the jason stuff            
            return new DropBoxAccountInfo(res);
        }

        public ICloudDirectoryEntry GetRoot(DropBoxSession session)
        {
            // build the root
            var root = new DropBoxDirectoryEntry(this, session, "");
            
            // fill up childs
			FillUpChilds(session, root);

            // return the root
			return root;			
        }

        public void FillUpChilds(DropBoxSession session, ICloudDirectoryEntry parent )
        {
            var resPath = "";
            
            var pe = parent as DropBoxDirectoryEntry;
            if (pe == null )
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidParameters);

            if (parent != null)
                resPath = pe.GetDropBoxPath();

            if (resPath == null)
                resPath = "";

            if (resPath.Length > 0)
                resPath = resPath.TrimStart('/');
            
			// build the metadata url
			String getMetaData;
			
			if (session.SandBoxMode )
                getMetaData = DropBoxSandboxRoot + EncodingHelper.UTF8Encode(resPath);
			else
                getMetaData = DropBoxDropBoxRoot + EncodingHelper.UTF8Encode(resPath);
                        
			// request the data from url           
            var res = session.RequestRessourceByUrl(getMetaData);
          
			// check error 
			if ( res.Length == 0 )
				throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotRetrieveDirectoryList);
			
            var dbentry = parent as DropBoxDirectoryEntry;            
            if ( !dbentry.BuildEntry(res) )
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotContactStorageService);
        }

        public ICloudDirectoryEntry CreateFolder(DropBoxSession session, String name, ICloudDirectoryEntry parent)
        {           
            // solve the parent issue
            if (parent == null)
            {
                parent = GetRoot(session);

                if (parent == null)
                    return null;
            }

            // get the dropbox object
            var dir = parent as DropBoxDirectoryEntry;

            // double check
            try
            {
                var childDir = parent.GetChild(name);
                if (childDir != null)
                    return childDir as ICloudDirectoryEntry;
            }
            catch (SharpBoxException e)
            {
                if (e.ErrorCode != SharpBoxErrorCodes.ErrorFileNotFound)
                    throw;
            }

			// build the directory path
			String path = dir.GetDropBoxPath() + "/" + name;
			
            // request the json object via oauth
            var parameters = new Dictionary<string, string>
            {
            	{ "path", path },
				{ "root", GetRootToken(session) }
            };

        	var res = session.RequestRessourceByUrl(DropBoxCreateFolder, parameters);
			if ( res.Length == 0 )
				return null;
			
            var newFolder = new DropBoxDirectoryEntry(this, session, parent, res);            
            return newFolder;
        }

        public Boolean DeleteItem(DropBoxSession session, ICloudFileSystemEntry target)
        {           
            // get the dropbox object
            var dir = target as DropBoxFileSystemEntry;

            // request the json object via oauth
            var parameters = new Dictionary<string, string>
            {
            	{ "path", dir.GetDropBoxPath() },
            	{ "root", GetRootToken(session) }
            };

        	try
            {
                session.RequestRessourceByUrl(DropBoxDeleteItem, parameters);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Boolean MoveItem(DropBoxSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {          
            // get the dropbox object
            var target = fsentry as DropBoxFileSystemEntry;
            var parent = newParent as DropBoxDirectoryEntry;

			if (target == null || parent == null)
				return false;

            // build the target 
            String toPath = parent.GetDropBoxPath().Length == 0 ? "/" + target.Name : parent.GetDropBoxPath() + "/" + target.Name;

            if (MoveOrRenameItem(session, target, toPath))
            {
                // set the new parent
                fsentry.Parent = newParent;
            }
            else
                return false;

            return true;
        }      

        public Boolean RenameItem(DropBoxSession session, ICloudFileSystemEntry fsentry, string newName)
        {
            // get the dropbox object
            var target = fsentry as DropBoxFileSystemEntry;

            // build the target path
            
            // check if we are the root object
            String dropBoxPath = target.GetDropBoxPath();
            if (dropBoxPath.Length == 0)
                return false;

            // build the renamed path
            PathHelper ph = new PathHelper(target.GetDropBoxPath());
            String toPath = ph.GetDirectoryName() + "/" + newName;            

            return MoveOrRenameItem(session, target, toPath);            
        }

        public ICloudFileSystemEntry CreateFile(DropBoxSession session, ICloudDirectoryEntry parent, String Name)
        {            
            // build the parent
            if (parent == null)
                parent = GetRoot(session);

            // build the file entry
            var newEntry = new DropBoxFileSystemEntry(this, session, Name, parent);
            return newEntry;
        }

        public Stream GetContentStream(DropBoxSession session, ICloudDirectoryEntry parent, String Name, FileAccess access )
        {
            // build url           
            String ur = GetDownloadFileUrl(session, parent);            

            // open the stream
            var fsStream = new DropBoxFileStream(session, ur, Name, access);
            return fsStream;            
        }

        private static String GetDownloadFileUrl(DropBoxSession session, ICloudDirectoryEntry parent)
        {
            // get the entry
            var p = parent as DropBoxDirectoryEntry;

            // gather information
            String rootToken = GetRootToken(session);
            String dropboxPath = p.GetDropBoxPath();

            // add all information to url;
            String url = DropBoxUploadDownloadFile + "/" + rootToken;

            if (dropboxPath.Length > 0 && dropboxPath[0] != '/')
                url += "/";

            url += EncodingHelper.UTF8Encode(dropboxPath);

            return url;
        }

		private static String GetRootToken(DropBoxSession session)
		{
			return session.SandBoxMode ? "sandbox" : "dropbox";
		}

    	private static bool MoveOrRenameItem(DropBoxSession session, DropBoxFileSystemEntry orgEntry, String toPath)
        {
            // request the json object via oauth
            var parameters = new Dictionary<string, string>
            {
            	{ "from_path", orgEntry.GetDropBoxPath() },
            	{ "root", GetRootToken(session) },
            	{ "to_path", toPath }
            };

    		try
            {
                var res = session.RequestRessourceByUrl(DropBoxMoveItem, parameters);

                // update the meta data
                orgEntry.BuildEntry(res);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
