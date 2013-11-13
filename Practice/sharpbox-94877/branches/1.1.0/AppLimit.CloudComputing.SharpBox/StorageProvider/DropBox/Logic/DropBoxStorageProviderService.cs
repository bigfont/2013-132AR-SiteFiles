﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Context;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using AppLimit.CloudComputing.SharpBox.Common.IO;
using AppLimit.CloudComputing.SharpBox.Common.Net;
using System.IO;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web;
using System.Net;
using System.Threading;

#if !WINDOWS_PHONE
using System.Web;
#endif

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.Logic
{
    internal class DropBoxStorageProviderService : GenericStorageProviderService
    {
        // server and base url 
        public const string DropBoxPrototcolPrefix = "https://";
        public const String DropBoxServer = "api.dropbox.com";
        public const String DropBoxEndUserServer = "www.dropbox.com";
        public const String DropBoxContentServer = "api-content.dropbox.com";
        public const String DropBoxBaseUrl = DropBoxPrototcolPrefix + DropBoxServer;
        public const String DropBoxBaseUrlEndUser = DropBoxPrototcolPrefix + DropBoxEndUserServer;

        // api version
        public const String DropBoxApiVersion = "0";

        public const String DropBoxGetAccountInfo = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/account/info";
        public const String DropBoxSandboxRoot = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/metadata/sandbox/";
        public const String DropBoxDropBoxRoot = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/metadata/dropbox/";
        public const String DropBoxCreateFolder = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/fileops/create_folder";
        public const String DropBoxDeleteItem = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/fileops/delete";
        public const String DropBoxMoveItem = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/fileops/move";

        public const String DropBoxUploadDownloadFile = DropBoxPrototcolPrefix + DropBoxContentServer + "/" + DropBoxApiVersion + "/files";

        public const String DropBoxMobileLogin = DropBoxBaseUrl + "/" + DropBoxApiVersion + "/token";

        #region DropBox Specific members

        public DropBoxAccountInfo GetAccountInfo(IStorageProviderSession session)
        {
            // request the json object via oauth            
            var res = DropBoxRequestParser.RequestResourceByUrl(DropBoxGetAccountInfo, this, session);

            // parse the jason stuff            
            return new DropBoxAccountInfo(res);
        }

        #endregion

        #region IStorageProviderService Members

        public override bool VerifyCredentialType(ICloudStorageCredentials credentials)
        {
            return (credentials is DropBoxCredentials); 
        }

        public override IStorageProviderSession CreateSession(ICloudStorageCredentials credentials, ICloudStorageConfiguration configuration)        
        {            
            // get the user credentials
            var userCred = credentials as DropBoxCredentials;
            var svcConfig = configuration as DropBoxConfiguration;
                
            // get the session
            return this.Authorize(userCred, svcConfig);            
        }

        public override IStorageProviderSession CreateSession(ICloudStorageAccessToken token, ICloudStorageConfiguration configuration)
        {
            // get the user credentials
            var userToken = token as DropBoxToken;
            var svcConfig = configuration as DropBoxConfiguration;

            // get the session
            return this.Authorize(userToken, svcConfig);
        }        

        public override ICloudFileSystemEntry RequestResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {            
            // build url
            String uriString = GetResourceUrlInternal(session, parent);

            if ( !Name.Equals("/") && Name.Length > 0 )
                uriString = PathHelper.Combine(uriString, HttpUtilityEx.UrlEncodeUTF8(Name));                

            // request the data from url           
            var res = DropBoxRequestParser.RequestResourceByUrl(uriString, this, session);

            // check error 
            if (res.Length == 0)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotRetrieveDirectoryList);

            // build the entry and subchilds
            BaseFileEntry entry = DropBoxRequestParser.CreateObjectsFromJsonString(res, this, session);
            
            // check if it was a deleted file
            if (entry.IsDeleted)
                return null;

            // set the parent
            entry.Parent = parent;

            // go ahead
            return entry;            
        }

        public override void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource)
        {
            // build url
            String uriString = GetResourceUrlInternal(session, resource);            

            // request the data from url           
            var res = DropBoxRequestParser.RequestResourceByUrl(uriString, this, session);

            // check error 
            if (res.Length == 0)
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotRetrieveDirectoryList);

            // build the entry and subchilds
            DropBoxRequestParser.UpdateObjectFromJsonString(res, resource as BaseFileEntry, this, session);            
        }

        public override ICloudFileSystemEntry CreateResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            // get the parent
            BaseDirectoryEntry parentDir = parent as BaseDirectoryEntry;

            // build new folder object
            var newFolder = new BaseDirectoryEntry(Name, 0, DateTime.Now, this, session);
            parentDir.AddChild(newFolder);

            // build the path for resource
            String resourcePath = GenericHelper.GetResourcePath(newFolder);
            
            // build the json parameters
            var parameters = new Dictionary<string, string>
            {
            	{ "path", resourcePath },
				{ "root", GetRootToken(session as DropBoxStorageProviderSession) }
            };

            // request the json object via oauth
            var res = DropBoxRequestParser.RequestResourceByUrl(DropBoxCreateFolder, parameters, this, session);
            if (res.Length == 0)
            {
                parentDir.RemoveChild(newFolder);
                return null;
            }
            else
            {
                // update the folder object
                DropBoxRequestParser.UpdateObjectFromJsonString(res, newFolder, this, session);
            }
            
            // go ahead
            return newFolder;            
        }

        public override bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            // build the path for resource
            String resourcePath = GenericHelper.GetResourcePath(entry);

            // request the json object via oauth
            var parameters = new Dictionary<string, string>
            {
            	{ "path", resourcePath },
            	{ "root", GetRootToken(session as DropBoxStorageProviderSession) }
            };

            try
            {
                // remove 
                DropBoxRequestParser.RequestResourceByUrl(DropBoxDeleteItem, parameters,this, session);

                // remove from parent
                BaseDirectoryEntry parentDir = entry.Parent as BaseDirectoryEntry;
                parentDir.RemoveChild(entry as BaseFileEntry);

            }
            catch (Exception)
            {
                return false;
            }

            return true;            
        }

        public override bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            // build the path for resource
            String resourcePath = GenericHelper.GetResourcePath(newParent);
            resourcePath = PathHelper.Combine(resourcePath, fsentry.Name);

            // Move
            if (MoveOrRenameItem(session as DropBoxStorageProviderSession, fsentry as BaseFileEntry, resourcePath))
            {
                // set the new parent
                fsentry.Parent = newParent;

                // go ahead
                return true;
            }
            else
                return false;
        }

        public override bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, string newName)
        {
            // build the path for resource
            String resourcePath = GenericHelper.GetResourcePath(fsentry.Parent);
            resourcePath = PathHelper.Combine(resourcePath, newName);
            
            // rename
            return MoveOrRenameItem(session as DropBoxStorageProviderSession, fsentry as BaseFileEntry, resourcePath);
        }             

        public override void StoreToken(IStorageProviderSession session, List<string> tokendata, ICloudStorageAccessToken token)
        {
            var dropboxToken = token as DropBoxToken;
            tokendata.Add(dropboxToken.TokenSecret);
            tokendata.Add(dropboxToken.TokenKey);
            tokendata.Add(dropboxToken.BaseCredentials.ConsumerKey);
            tokendata.Add(dropboxToken.BaseCredentials.ConsumerSecret);
        }

        public override ICloudStorageAccessToken LoadToken(IStorageProviderSession session, List<string> tokendata)
        {
            String type = tokendata[0];

            if (!type.Equals(typeof(DropBoxToken).ToString()))
                throw new InvalidCastException("Token type not supported through this provider");

            var tokenSecret = tokendata[1];
            var tokenKey = tokendata[2];

            DropBoxBaseCredentials bc = new DropBoxBaseCredentials();
            bc.ConsumerKey = tokendata[3];
            bc.ConsumerSecret = tokendata[4];

            return new DropBoxToken(tokenKey, tokenSecret, bc);          
        }

        public override string GetResourceUrl(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, String additionalPath)
        {
            // get the dropbox session
            DropBoxStorageProviderSession dbSession = session as DropBoxStorageProviderSession;

            // build the internal url 
            String url = GetResourceUrlInternal(session, fileSystemEntry);

            // add the optional path
            if (additionalPath != null)
                url = PathHelper.Combine(url, HttpUtilityEx.UrlEncodeUTF8(additionalPath));

            // generate the oauth url
            OAuthService svc = new OAuthService();
            return svc.GetProtectedResourceUrl(url, dbSession.Context, dbSession.SessionToken as DropBoxToken, null, ValidRequestMethod.Get);            
        }
       
        public override Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry)
        {
            // build the url 
            string url = GetDownloadFileUrlInternal(session, fileSystemEntry);

            // build the service
            OAuthService svc = new OAuthService();

            // get the dropbox session
            DropBoxStorageProviderSession dropBoxSession = session as DropBoxStorageProviderSession;

            // create webrequst 
            WebRequest requestProtected = svc.CreateWebRequest(url, ValidRequestMethod.Get, null, null, dropBoxSession.Context, (DropBoxToken)dropBoxSession.SessionToken, null);
            
            // get the response
            WebResponse response = svc.GetWebResponse(requestProtected);

            // get the data stream
            Stream orgStream = svc.GetResponseStream(response);            

            // build the download stream
            BaseFileEntryDownloadStream dStream = new BaseFileEntryDownloadStream(orgStream, fileSystemEntry);

            // put the disposable on the stack
            dStream._DisposableObjects.Push(response);

            // go ahead
            return dStream;
        }

        public override Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, long uploadSize)
        {            
            // build the url 
            string url = GetDownloadFileUrlInternal(session, fileSystemEntry.Parent);

            // get the session
            DropBoxStorageProviderSession dbSession = session as DropBoxStorageProviderSession;

            // build service
            OAuthService svc = new OAuthService();
            
            // encode the filename
            String fileName = fileSystemEntry.Name;            

            // build oAuth parameter
            var param = new Dictionary<string, string>();
            param.Add("file", fileName);

            // sign the url 
            String signedUrl = svc.GetSignedUrl(url, dbSession.Context, dbSession.SessionToken as DropBoxToken, param);

            // build upload web request
            WebRequest uploadRequest = svc.CreateWebRequestMultiPartUpload(signedUrl, null);

            // get the network stream
            WebRequestStream ws = svc.GetRequestStreamMultiPartUpload(uploadRequest, fileName, uploadSize);

            // register the post dispose opp
            ws.PushPostDisposeOperation(CommitUploadStream, svc, uploadRequest, uploadSize, fileSystemEntry);

            // go ahead
            return ws;
        }

        public void CommitUploadStream(params object[] arg)
        {            
            // convert the args
            OAuthService svc = arg[0] as OAuthService;
            HttpWebRequest uploadRequest = arg[1] as HttpWebRequest;
            long uploadSize = (long)arg[2];
            BaseFileEntry fileSystemEntry = arg[3] as BaseFileEntry;

            // perform the request
            HttpStatusCode code;
            WebException e;
            svc.PerformWebRequest2(uploadRequest, null, out code, out e);

            // check the ret value
            if (code != HttpStatusCode.OK)
                SharpBoxException.ThrowSharpBoxExceptionBasedOnHttpErrorCode(uploadRequest, code, e);                            

            // set the length
            fileSystemEntry.Length = uploadSize;
        }       

        #endregion

        #region Authorization Helper

        private DropBoxStorageProviderSession Authorize(DropBoxCredentials credentials, DropBoxConfiguration configuration)
        {
            // Get a valid dropbox session through oAuth authorization
            DropBoxStorageProviderSession session = AuthorizeAndGetSession(credentials, configuration);

            //( Get a valid root object
            ICloudDirectoryEntry root = GetRootBySession(session);
			
			// return the infos
			return root == null ? null : session;
        }

        public DropBoxStorageProviderSession Authorize(DropBoxToken token, DropBoxConfiguration configuration)
        {
            // Get a valid dropbox session through oAuth authorization
            DropBoxStorageProviderSession session = BuildSessionFromAccessToken(token, configuration);

            //( Get a valid root object
            ICloudDirectoryEntry root = GetRootBySession(session);

            // return the infos
            return root == null ? null : session;
        }

        private ICloudDirectoryEntry GetRootBySession(DropBoxStorageProviderSession session)
        {
            // now check if we have application keys and secrets from a sandbox or a fullbox
            // try to load the root of the full box if this fails we have only sandbox access
            ICloudDirectoryEntry root = RequestResource(session, "/", null) as ICloudDirectoryEntry;
            if (root == null)
            {
                // disbale sandbox mode
                session.SandBoxMode = true;

                // retry to get root object
                root = RequestResource(session, "/", null) as ICloudDirectoryEntry;
            }

            // go ahead
            return root;
        }

        private DropBoxStorageProviderSession AuthorizeAndGetSession(DropBoxCredentials credentials, DropBoxConfiguration configuration)
        {         
            // build the consumer context
            var consumerContext = new OAuthConsumerContext(credentials.ConsumerKey, credentials.ConsumerSecret);

            // build up the oauth session
			var serviceContext = new OAuthServiceContext(DropBoxConfiguration.RequestTokenUrl.ToString(),
                DropBoxConfiguration.AuthorizationTokenUrl.ToString(), configuration.AuthorizationCallBack.ToString(),
				DropBoxConfiguration.AccessTokenUrl.ToString());

            // get a request token from the provider      
            OAuthService svc = new OAuthService();
            var oAuthRequestToken = svc.GetRequestToken(serviceContext, consumerContext);
            DropBoxToken DropBoxRequestToken = new DropBoxToken(oAuthRequestToken, credentials);

            // build up a request Token Session
            var requestSession = new DropBoxStorageProviderSession(DropBoxRequestToken, configuration, consumerContext, this);

            // build up the parameters
            var param = new Dictionary<String, String>
            {
            	{ "email", credentials.UserName }, 
				{ "password", credentials.Password }
            };

        	// call the mobile login api 
            String result = "";

            try
            {
                result = DropBoxRequestParser.RequestResourceByUrl(DropBoxMobileLogin, param, this, requestSession);
                if (result.Length == 0)
                    throw new UnauthorizedAccessException();
            }

#if MONOTOUCH || WINDOWS_PHONE || ANDROID
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                    throw ex;
                else
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotContactStorageService, ex);
            }
#else
            catch (System.Web.HttpException netex) 
            {
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotContactStorageService, netex);
            }
#endif

            // exchange a request token for an access token
            var accessToken = new DropBoxToken(result);

            // adjust the token 
            if (accessToken.BaseCredentials == null)
            {
                accessToken.BaseCredentials = new DropBoxBaseCredentials();
                accessToken.BaseCredentials.ConsumerKey = credentials.ConsumerKey;
                accessToken.BaseCredentials.ConsumerSecret = credentials.ConsumerSecret;
            }


            // build the session
            return BuildSessionFromAccessToken(accessToken, configuration);            
        }

        public DropBoxStorageProviderSession BuildSessionFromAccessToken(DropBoxToken token, DropBoxConfiguration configuration)
        {            
            // build the consumer context
            var consumerContext = new OAuthConsumerContext(token.BaseCredentials.ConsumerKey, token.BaseCredentials.ConsumerSecret);
                        
            // build the session
            var session = new DropBoxStorageProviderSession(token, configuration, consumerContext, this);

            // go aahead
            return session;
        }

        private static String GetRootToken(DropBoxStorageProviderSession session)
        {
            return session.SandBoxMode ? "sandbox" : "dropbox";
        }

        private bool MoveOrRenameItem(DropBoxStorageProviderSession session, BaseFileEntry orgEntry, String toPath)
        {
            // build the path for resource
            String resourcePath = GenericHelper.GetResourcePath(orgEntry);

            // request the json object via oauth
            var parameters = new Dictionary<string, string>
            {
            	{ "from_path", resourcePath },
            	{ "root", GetRootToken(session) },
            	{ "to_path", toPath }
            };

            try
            {
                // move or rename the entry
                var res = DropBoxRequestParser.RequestResourceByUrl(DropBoxMoveItem, parameters, this, session);

                // update the entry
                DropBoxRequestParser.UpdateObjectFromJsonString(res, orgEntry, this, session);                               
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }    

        private string GetResourceUrlInternal(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry)
        {
            // cast varibales
            DropBoxStorageProviderSession dropBoxSession = session as DropBoxStorageProviderSession;

            // build the path for resource
            String resourcePath = GenericHelper.GetResourcePath(fileSystemEntry);

            // trim heading and trailing slashes
            resourcePath = resourcePath.TrimStart('/');
            resourcePath = resourcePath.TrimEnd('/');

            // build the metadata url
            String getMetaData;

            // get url
            if (dropBoxSession.SandBoxMode)
                getMetaData = PathHelper.Combine(DropBoxSandboxRoot, HttpUtilityEx.UrlEncodeUTF8(resourcePath)); 
            else
                getMetaData = PathHelper.Combine(DropBoxDropBoxRoot, HttpUtilityEx.UrlEncodeUTF8(resourcePath));

            return getMetaData;
        }

        public static String GetDownloadFileUrlInternal(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            // cast varibales
            DropBoxStorageProviderSession dropBoxSession = session as DropBoxStorageProviderSession;

            // gather information
            String rootToken = GetRootToken(dropBoxSession);
            String dropboxPath = GenericHelper.GetResourcePath(entry);

            // add all information to url;
            String url = DropBoxUploadDownloadFile + "/" + rootToken;

            if (dropboxPath.Length > 0 && dropboxPath[0] != '/')
                url += "/";

            url += HttpUtilityEx.UrlEncodeUTF8(dropboxPath);

            return url;
        }                     
      
        #endregion
    }
}