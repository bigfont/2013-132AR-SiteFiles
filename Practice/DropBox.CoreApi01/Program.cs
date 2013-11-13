using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DropBox.CoreApi01
{
    public class Share
    { 
        public string url;
        public string expires;           
    }

    public class Content
    {
        public string size;
        public string rev;
        public string thumb_exists;
        public string bytes;
        public string modified;
        public string client_mtime;
        public string path;
        public string is_dir;
        public string icon;
        public string root;
        public string mime_type;
        public string revision;
    }
    public class Metadata
    {
        public string size;
        public string rev;
        public string thumb_exists;
        public string bytes;
        public string modified;
        public string client_mtime;
        public string path;
        public string is_dir;
        public string icon;
        public string root;
        public Content[] contents;
        public string mime_type;
        public string revision;
    }
    public class AccountInfo
    {
        public string referral_link;
        public string display_name;
        public string uid;
        public string country;
        public QuotaInfo quota_info;
        public string email;
    }

    public class QuotaInfo
    {
        public string datastores;
        public string shared;
        public string quota;
        public string normal;
    }

    public class MyProgram
    {
        private static string BEARER_CODE = "SxQS0Gl4VgQAAAAAAAAAAUKQZEZN088vu-NY0rA-vnALIsut1e0aFTSAK7oePPt8";
        private static string ACCOUNT_INFO = "https://api.dropbox.com/1/account/info";
        private static string METADATA = "https://api.dropbox.com/1/metadata/sandbox?list=true";
        private static string SHARES = "https://api.dropbox.com/1/shares/sandbox";

        public static string AUTH_HEADER { get { return "Bearer " + BEARER_CODE; } }
        public static void Main(string[] args)
        {
            GetShare();
            GetAccountInfo();
            GetMetadata();
            Console.ReadLine();
        }

        public static Share GetShare()
        { 
            string json;
            Share shares; 
            
            json = RequestResponse(SHARES, AUTH_HEADER);
            shares = JsonConvert.DeserializeObject<Share>(json);

            return shares;
        }

        public static Metadata GetMetadata()
        {
            string json;
            Metadata metadata;

            json = RequestResponse(METADATA, AUTH_HEADER);
            metadata = JsonConvert.DeserializeObject<Metadata>(json);

            return metadata;
        }

        public static AccountInfo GetAccountInfo()
        {
            string json;
            AccountInfo accountInfo;

            json = RequestResponse(ACCOUNT_INFO, AUTH_HEADER);
            accountInfo = JsonConvert.DeserializeObject<AccountInfo>(json);

            return accountInfo;
        }

        public static string RequestResponse(string url, string authHeader)
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
