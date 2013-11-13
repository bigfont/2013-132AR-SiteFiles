using System;

using Newtonsoft.Json.Linq;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
    /*
     *  {
     *      “shared”: 37378890,
     *      “quota”: 62277025792,
     *      “normal”: 263758550
     *  }
     */

    internal class DropBoxQuotaInfo
    {
        public readonly UInt64 SharedBytes;
        public readonly UInt64 QuotaBytes;
        public readonly UInt64 NormalBytes;

        public DropBoxQuotaInfo(String jmstext)
        {
            var quotainfo = JObject.Parse(jmstext);

            SharedBytes = (UInt64)quotainfo.SelectToken("shared");
            QuotaBytes = (UInt64)quotainfo.SelectToken("quota");
            NormalBytes = (UInt64)quotainfo.SelectToken("normal");            
        }
    }
}
