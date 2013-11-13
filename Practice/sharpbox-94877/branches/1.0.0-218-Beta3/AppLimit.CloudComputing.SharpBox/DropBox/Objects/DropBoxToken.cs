using System;
using AppLimit.CloudComputing.SharpBox.OAuth.Token;
using Newtonsoft.Json.Linq;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
	internal class DropBoxToken : OAuthToken
	{
		public DropBoxToken (String jsonString)
			: base("","")
		{
			var tokeInfo = JObject.Parse(jsonString);

            this.TokenSecret     = (string)tokeInfo.SelectToken("secret");
            this.TokenKey		 = (string)tokeInfo.SelectToken("token");            	
		}
	}
}

