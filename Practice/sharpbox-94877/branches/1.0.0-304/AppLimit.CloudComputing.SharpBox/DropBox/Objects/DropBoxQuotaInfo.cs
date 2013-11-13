using System;
using AppLimit.CloudComputing.SharpBox.Common;


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
			JsonHelper jh = new JsonHelper();
            if (jh.ParseJsonMessage(jmstext))
			{
            	SharedBytes = Convert.ToUInt64(jh.GetProperty("shared"));
            	QuotaBytes = Convert.ToUInt64(jh.GetProperty("quota"));
            	NormalBytes = Convert.ToUInt64(jh.GetProperty("normal"));
			}
        }
    }
}
