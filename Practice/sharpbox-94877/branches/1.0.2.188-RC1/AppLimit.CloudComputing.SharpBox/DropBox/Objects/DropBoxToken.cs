using System;
using AppLimit.CloudComputing.OAuth.Token;
using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
	internal class DropBoxToken : OAuthToken, ICloudStorageAccessToken
	{
		public DropBoxToken(String jsonString)
			: base("", "")
		{
			var jh = new JsonHelper();
			if (jh.ParseJsonMessage(jsonString))
			{
				TokenSecret = jh.GetProperty("secret");
				TokenKey = jh.GetProperty("token");
			}
		}

		public DropBoxToken(OAuthToken token)
			: base(token.TokenKey, token.TokenSecret)
		{
		}

		public DropBoxToken(string tokenKey, string tokenSecret)
			: base(tokenKey, tokenSecret)
		{
		}
	}
}

