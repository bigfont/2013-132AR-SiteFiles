using System;
using AppLimit.CloudComputing.SharpBox.Common;

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
			JsonHelper jh = new JsonHelper();
			if (!jh.ParseJsonMessage(jmstext))
				return;
			            
            Country     = jh.GetProperty("country");
            DisplayName = jh.GetProperty("display_name");
            UserId      = jh.GetPropertyInt("uid");
            
			string quotainfo = jh.GetSubObjectString("quota_info");
            if ( quotainfo != null )
                QuotaInfo = new DropBoxQuotaInfo(quotainfo.ToString());
        }        
    }
}
