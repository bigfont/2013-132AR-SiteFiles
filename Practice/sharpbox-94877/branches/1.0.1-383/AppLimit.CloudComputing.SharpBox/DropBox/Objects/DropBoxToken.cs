using System;
using AppLimit.CloudComputing.OAuth.Token;
using AppLimit.CloudComputing.SharpBox.Common;

namespace AppLimit.CloudComputing.SharpBox.DropBox.Objects
{
	internal class DropBoxToken : OAuthToken
	{
		public DropBoxToken (String jsonString)
			: base("","")
		{
			JsonHelper jh = new JsonHelper();
			if ( jh.ParseJsonMessage(jsonString) )
			{
				this.TokenSecret = jh.GetProperty("secret");
				this.TokenKey = jh.GetProperty("token");
			}						
		}
	}
}

