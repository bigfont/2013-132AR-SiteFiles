using AspDropBox.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AspDropBox.Core
{
    public enum RootType
    { 
        Sandbox, 
        DropBox
    }
    public class DropBoxCoreApi
    {
        private const string ACCOUNT_INFO = "https://api.dropbox.com/1/account/info";
        private const string METADATA = "https://api.dropbox.com/1/metadata/<root>/<path>?list=true";
        private const string SHARES = "https://api.dropbox.com/1/shares/<root>/<path>";        
        private string bearerCode;
        private string authHeader { get { return "Bearer " + bearerCode; } }

        public DropBoxCoreApi(string bearerCode)
        {            
            this.bearerCode = bearerCode;
        }

        public Share GetShare(RootType rootType, string path = "")
        {
            string json;
            string url;
            Share shares;

            // update the url
            url = SHARES
                .Replace("<root>", rootType.ToString().ToLower())
                .Replace("<path>", path.ToLower());

            json = RequestResponse(url, authHeader);
            shares = JsonConvert.DeserializeObject<Share>(json);

            return shares;
        }
        public Metadata GetMetadata(RootType rootType, string path = "")
        {
            string json;
            string url;
            Metadata metadata;

            // update the url
            url = METADATA
                .Replace("<root>", rootType.ToString().ToLower())
                .Replace("<path>", path.ToLower());                

            json = RequestResponse(url, authHeader);
            metadata = JsonConvert.DeserializeObject<Metadata>(json);

            return metadata;
        }
        public AccountInfo GetAccountInfo()
        {
            string json;
            AccountInfo accountInfo;

            json = RequestResponse(ACCOUNT_INFO, authHeader);
            accountInfo = JsonConvert.DeserializeObject<AccountInfo>(json);

            return accountInfo;
        }
        private string RequestResponse(string url, string authHeader)
        {
            string text;
            WebRequest request;
            WebResponse response;

            request = WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.Authorization, authHeader);
            request.ContentType = "application/json";

            response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}
