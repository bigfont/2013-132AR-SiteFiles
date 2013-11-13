using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.OAuth.Context
{
    public class OAuthServiceContext
    {
        public readonly String RequestTokenUrl;
        public readonly String AuthorizationUrl;
        public readonly String CallbackUrl;
        public readonly String AccessTokenUrl;

        public OAuthServiceContext(String RequestTokenUrl, String AuthorizationUrl, String CallbackUrl, String AccessTokenUrl)
        {
            this.RequestTokenUrl = RequestTokenUrl;
            this.AuthorizationUrl = AuthorizationUrl;
            this.CallbackUrl = CallbackUrl;
            this.AccessTokenUrl = AccessTokenUrl;
        }
    }
}
