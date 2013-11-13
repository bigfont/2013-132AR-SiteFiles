using System;

using Newtonsoft.Json.Linq;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
 /*    
  * {
  *     “country”: “”,
  *     “display_name”: “John Q. User”,
  *     “quota_info”: 
  *         {
  *             “shared”: 37378890,
  *             “quota”: 62277025792,
  *             “normal”: 263758550
  *         },
  *     “uid”: “174”
  * }
  */ 
    internal class DropBoxAccountInfo
    {
        public readonly String              Country;
        public readonly String              DisplayName;
        public readonly Int32               UserId;
        public readonly DropBoxQuotaInfo    QuotaInfo;

        public DropBoxAccountInfo(String jmstext)
        {
            var accountInfo = JObject.Parse(jmstext);

            Country     = (string)accountInfo.SelectToken("country");
            DisplayName = (string)accountInfo.SelectToken("display_name");
            UserId      = (Int32)accountInfo.SelectToken("uid");
            
            var quotainfo = accountInfo.SelectToken("quota_info");
            if ( quotainfo != null )
                QuotaInfo = new DropBoxQuotaInfo(quotainfo.ToString());
        }        
    }
}
