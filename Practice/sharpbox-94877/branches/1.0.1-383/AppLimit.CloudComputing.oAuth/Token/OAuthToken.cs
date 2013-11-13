using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.OAuth.Token
{
    public class OAuthToken
    {
        public String TokenKey { get; protected set; }
        public String TokenSecret { get; protected set; }

        public OAuthToken(String TokenKey, String TokenSecret)
        {
            this.TokenKey = TokenKey;
            this.TokenSecret = TokenSecret;
        }
    }
}
