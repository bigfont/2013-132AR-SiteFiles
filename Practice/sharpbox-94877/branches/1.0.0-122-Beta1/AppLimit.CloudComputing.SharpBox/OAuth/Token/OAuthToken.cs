using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox.OAuth.Token
{
    internal class OAuthToken
    {
        public readonly String TokenKey;
        public readonly String TokenSecret;

        internal OAuthToken(String TokenKey, String TokenSecret)
        {
            this.TokenKey = TokenKey;
            this.TokenSecret = TokenSecret;
        }
    }
}
