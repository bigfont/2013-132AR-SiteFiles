using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Web;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common;

using AppLimit.CloudComputing.OAuth;
using AppLimit.CloudComputing.OAuth.Context;
using AppLimit.CloudComputing.OAuth.Token;
using AppLimit.CloudComputing.SharpBox.DropBox.Objects;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Logic
{
    internal class DropBoxApplication
    {
        private const String _dropBoxCallbackUrl = "http://www.applimit.com";

        // server and base url 
        private const String _dropBoxServer = "api.dropbox.com";
        private const String _dropBoxContentServer = "api-content.dropbox.com";
        private const String _dropBoxBaseUrl = "http://" + _dropBoxServer;

        // api version
        private const String _dropBoxApiVersion = "0";
                
        private const String _dropBoxGetAccountInfo = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/account/info";
        private const String _dropBoxSandboxRoot = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/metadata/sandbox/";
		private const String _dropBoxDropBoxRoot = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/metadata/dropbox/";
        private const String _dropBoxCreateFolder = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/fileops/create_folder";
        private const String _dropBoxDeleteItem = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/fileops/delete";
        private const String _dropBoxMoveItem = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/fileops/move";
        
        private const String _dropBoxUploadDownloadFile = "http://" + _dropBoxContentServer + "/" + _dropBoxApiVersion + "/files";        

		private const String _dropBoxMobileLogin = "https://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/token";
		
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
            // build the consumer context
            var consumerContext = new OAuthConsumerContext(_consumerKey, _consumerSecret);                                                                                                               

            // build up the oauth session
            var serviceContext = new OAuthServiceContext(   _service.Configuration.GetRequestTokenUrl().ToString(),
                                                            _service.Configuration.GetAuthorizationTokenUrl().ToString(),
                                                            _dropBoxCallbackUrl,
                                                            _service.Configuration.GetAccessTokenUrl().ToString());            

             // get a request token from the provider
            var requestToken = OAuthService.GetRequestToken(serviceContext, consumerContext);

			
			// build up a request Token Session
			DropBoxSession requestSession = new DropBoxSession(requestToken, consumerContext);
			
			// build up the parameters
			Dictionary<String, String> param = new Dictionary<String, String>();
			param.Add("email", userName);
			param.Add("password", password);
			
			// call the mobile login api 
			String result = requestSession.RequestRessourceByUrl(_dropBoxMobileLogin, param);
            if (result.Length == 0)
                return null;
		
            // exchange a request token for an access token
            var accessToken = new DropBoxToken(result);
				
			// build the session
			DropBoxSession session = new DropBoxSession(accessToken, consumerContext);            
			
			// now check if we have application keys and secrets from a sandbox or a fullbox
			// try to load the root of the sandbox if this fails we have full access
            ICloudDirectoryEntry root = GetRoot(session);
			if ( root == null ) 
			{
				// disbale sandbox mode
				session.bSandBoxMode = false;
				
				// retry to get root object
				root = GetRoot(session);
			}
			
			// return the infos
			return root == null ? null : session;
        }

        public void Close(DropBoxSession session)
        {            
            
        }

        public DropBoxAccountInfo GetAccountInfo(DropBoxSession session)
        {
            // request the json object via oauth            
            var res = session.RequestRessourceByUrl(_dropBoxGetAccountInfo);

            // parse the jason stuff            
            return new DropBoxAccountInfo(res);
        }

        public ICloudDirectoryEntry GetRoot(DropBoxSession session)
        {
            var root = new DropBoxDirectoryEntry(this, session as DropBoxSession, "");
            
			if ( FillUpChilds(session, root) )
				return root;
			else
				return null;
        }

        public Boolean FillUpChilds(DropBoxSession session, ICloudDirectoryEntry parent )
        {
            var resPath = "";
            
            var pe = parent as DropBoxDirectoryEntry;
            if (pe == null )
                return false;

            if (parent != null)
                resPath = pe.GetDropBoxPath();

            if (resPath == null)
                resPath = "";

            if (resPath.Length > 0)
                resPath = resPath.TrimStart('/');
            
			// build the metadata url
			String getMetaData;
			
			if (session.bSandBoxMode )
                getMetaData = _dropBoxSandboxRoot + EncodingHelper.UTF8Encode(resPath);
			else
                getMetaData = _dropBoxDropBoxRoot + EncodingHelper.UTF8Encode(resPath);
                        
			// request the data from url           
            var res = session.RequestRessourceByUrl(getMetaData);
          
			// check error 
			if ( res.Length == 0 )
				return false;
			
            var dbentry = parent as DropBoxDirectoryEntry;
            if (dbentry == null)
				return false;
			
            return dbentry.BuildEntry(res);           
        }

        public ICloudDirectoryEntry CreateFolder(DropBoxSession session, String Name, ICloudDirectoryEntry parent)
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
            if (parent.GetChild(Name) != null)
                return null;

			// build the directory path
			String path = dir.GetDropBoxPath() + "/" + Name;
			
            // request the json object via oauth
            Dictionary<String, String> parameters = new Dictionary<string, string>();
                        
            parameters.Add("path", path);						
            parameters.Add("root", GetRootToken(session));

            var res = session.RequestRessourceByUrl(_dropBoxCreateFolder, parameters);
			if ( res.Length == 0 )
				return null;
			
            var newFolder = new DropBoxDirectoryEntry(this, session as DropBoxSession, parent, res);            
            return newFolder;
        }

        public Boolean DeleteItem(DropBoxSession session, ICloudFileSystemEntry target)
        {           
            // get the dropbox object
            var dir = target as DropBoxFileSystemEntry;

            // request the json object via oauth
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters.Add("path", dir.GetDropBoxPath());
            parameters.Add("root", GetRootToken(session));

            try
            {
                session.RequestRessourceByUrl(_dropBoxDeleteItem, parameters);
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
            DropBoxFileSystemEntry newEntry = new DropBoxFileSystemEntry(this, session, Name, parent);
            return newEntry;
        }

        public Stream GetContentStream(DropBoxSession session, ICloudDirectoryEntry parent, String Name, FileAccess access )
        {
            // build url           
            String ur = GetDownloadFileUrl(session, parent);            

            // open the stream
            DropBoxFileStream fsStream = new DropBoxFileStream(session, ur, Name, access);
            return fsStream;            
        }

        private String GetDownloadFileUrl(DropBoxSession session, ICloudDirectoryEntry parent)
        {
            // get the entry
            var p = parent as DropBoxDirectoryEntry;

            // gather information
            String rootToken = GetRootToken(session);
            String dropboxPath = p.GetDropBoxPath();

            // add all information to url;
            String url = _dropBoxUploadDownloadFile + "/" + rootToken;

            if (dropboxPath.Length > 0 && dropboxPath[0] != '/')
                url += "/";

            url += dropboxPath;

            return url;
        }

		private String GetRootToken(DropBoxSession session)
		{
			if ( session.bSandBoxMode )
				return "sandbox";
			else
				return "dropbox";
		}

        private bool MoveOrRenameItem(DropBoxSession session, DropBoxFileSystemEntry orgEntry, String toPath)
        {
            // request the json object via oauth
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters.Add("from_path", orgEntry.GetDropBoxPath());
            parameters.Add("root", GetRootToken(session));
            parameters.Add("to_path", toPath);

            try
            {
                var res = session.RequestRessourceByUrl(_dropBoxMoveItem, parameters);

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
