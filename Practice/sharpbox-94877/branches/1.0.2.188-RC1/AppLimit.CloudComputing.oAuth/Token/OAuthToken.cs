using System;

namespace AppLimit.CloudComputing.OAuth.Token
{
    public class OAuthToken
    {
        public String TokenKey { get; protected set; }
        public String TokenSecret { get; protected set; }

        public OAuthToken(String tokenKey, String tokenSecret)
        {
            TokenKey = tokenKey;
            TokenSecret = tokenSecret;
        }
    }
}
