using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

using AppLimit.CloudComputing.SharpBox;

using AppLimit.CloudComputing.SharpBox.OAuth;
using AppLimit.CloudComputing.SharpBox.OAuth.Context;
using AppLimit.CloudComputing.SharpBox.OAuth.Token;
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
        private const String _dropBoxCreateFolder = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/fileops/create_folder";
        private const String _dropBoxDeleteItem = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/fileops/delete";
        private const String _dropBoxMoveItem = "http://" + _dropBoxServer + "/" + _dropBoxApiVersion + "/fileops/move";
        
        private const String _dropBoxUploadDownloadFile = "http://" + _dropBoxContentServer + "/" + _dropBoxApiVersion + "/files/sandbox";        

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

        public DropBoxSession Authorize(String userName, String password, Boolean ShowHidden )
        {   						
            // build the consumer context
            var consumerContext = new OAuthConsumerContext(_consumerKey, _consumerSecret);                                                                                                               

            // build up the oauth session
            var serviceContext = new OAuthServiceContext(   _service.Configuration.GetRequestTokenUrl().ToString(),
                                                            _service.Configuration.GetAuthorizationTokenUrl().ToString(),
                                                            "http://www.applimit.com",
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
						
            // exchange a request token for an access token
            var accessToken = new DropBoxToken(result);
            
            return accessToken == null ? null : new DropBoxSession(accessToken, consumerContext);            
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
            FillUpChilds(session, root);

            return root;
        }

        public void FillUpChilds(DropBoxSession session, ICloudDirectoryEntry parent )
        {
            var resPath = "";
            
            var pe = parent as DropBoxDirectoryEntry;
            if (pe == null )
                return;

            if (parent != null)
                resPath = pe.GetDropBoxPath();

            if (resPath == null)
                resPath = "";

            if (resPath.Length > 0)
                resPath = resPath.TrimStart('/');


            var getMetaData = new Uri(_dropBoxSandboxRoot + resPath);

            var res = session.RequestRessourceByUrl(getMetaData.ToString());
          
            var dbentry = parent as DropBoxDirectoryEntry;
            if (dbentry != null)
            {
                dbentry.BuildEntry(res);
            }            
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

            // request the json object via oauth
            Dictionary<String, String> parameters = new Dictionary<string, string>();
                        
            parameters.Add("path", dir.GetDropBoxPath() + "/" + Name );            
            parameters.Add("root", "sandbox");

            var res = session.RequestRessourceByUrl(_dropBoxCreateFolder, parameters);

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
            parameters.Add("root", "sandbox");

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

            // request the json object via oauth
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            parameters.Add("from_path", target.GetDropBoxPath());
            parameters.Add("root", "sandbox");
            parameters.Add("to_path", parent.GetDropBoxPath().Length == 0 ? "/" + target.Name : parent.GetDropBoxPath() + "/" + target.Name );

            try
            {
                var res = session.RequestRessourceByUrl(_dropBoxMoveItem, parameters);

                // set the new parent
                fsentry.Parent = newParent;

                // update the meta data
                target.BuildEntry(res);
            }
            catch (Exception)
            {
                return false;
            }
                        
            return true;
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
            var p = parent as DropBoxDirectoryEntry;
            String ur = _dropBoxUploadDownloadFile + "/" + p.GetDropBoxPath();

            // open the stream
            DropBoxFileStream fsStream = new DropBoxFileStream(session, ur, Name, access);
            return fsStream;            
        }
    }
}
