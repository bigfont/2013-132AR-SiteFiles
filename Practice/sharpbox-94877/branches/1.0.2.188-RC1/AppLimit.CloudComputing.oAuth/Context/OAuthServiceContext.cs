using System;

namespace AppLimit.CloudComputing.OAuth.Context
{
    public class OAuthServiceContext
    {
        public readonly String RequestTokenUrl;
        public readonly String AuthorizationUrl;
        public readonly String CallbackUrl;
        public readonly String AccessTokenUrl;

        public OAuthServiceContext(String requestTokenUrl, String authorizationUrl, String callbackUrl, String accessTokenUrl)
        {
            RequestTokenUrl = requestTokenUrl;
            AuthorizationUrl = authorizationUrl;
            CallbackUrl = callbackUrl;
            AccessTokenUrl = accessTokenUrl;
        }
    }
}
