using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using AppLimit.CloudComputing.SharpBox.OAuth.Context;
using AppLimit.CloudComputing.SharpBox.OAuth.Token;

namespace AppLimit.CloudComputing.SharpBox.OAuth.Impl
{
    internal class OAuthUrlGenerator
    {
        static public String GenerateRequestTokenUrl(String BaseRequestTokenUrl, OAuthConsumerContext context)
        {
            return GenerateSignedUrl(BaseRequestTokenUrl, "GET", context, null);            
        }

        static public String GenerateAuthorizationUrl(String BaseAuthorizeUrl, String CallbackUrl, OAuthToken requestToken)
        {
            StringBuilder sb = new StringBuilder(BaseAuthorizeUrl);
            
            sb.AppendFormat("?oauth_token={0}", HttpUtility.UrlEncode(requestToken.TokenKey));
            sb.AppendFormat("&oauth_callback={0}", HttpUtility.UrlEncode(CallbackUrl.ToString()));

            return sb.ToString();
        }

        static public String GenerateAccessTokenUrl(String BaseAccessTokenUrl, OAuthConsumerContext context, OAuthToken requestToken)
        {
            return GenerateSignedUrl(BaseAccessTokenUrl, "GET", context, requestToken);            
        }

        static private String GenerateSignedUrl(String BaseRequestTokenUrl, String method, OAuthConsumerContext context, OAuthToken token)
        {
            String normalizedUrl;
            String normalizedRequestParameters;

            OAuthBase oAuth = new OAuthBase();

            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string sig = oAuth.GenerateSignature(   new Uri(BaseRequestTokenUrl),
                                                    context.ConsumerKey, context.ConsumerSecret,
                                                    token == null ? string.Empty : token.TokenKey, token == null ? string.Empty : token.TokenSecret,
                                                    method, timeStamp, nonce,
                                                    context.SignatureMethod,
                                                    out normalizedUrl, out normalizedRequestParameters);

            sig = HttpUtility.UrlEncode(sig);

            StringBuilder sb = new StringBuilder(normalizedUrl);

            sb.AppendFormat("?");
            sb.AppendFormat(normalizedRequestParameters);
            sb.AppendFormat("&oauth_signature={0}", sig);

            return sb.ToString();
        }

        static public String GenerateSignedUrl(String BaseRessourceUrl, String method, OAuthConsumerContext context, OAuthToken token, Dictionary<String, String> urlParameter)
        {            
            StringBuilder sb = new StringBuilder(BaseRessourceUrl);

            if (urlParameter != null)
            {
                sb.Append('?');

                foreach (KeyValuePair<String, String> kvp in urlParameter)
                {   
                    if ( sb[sb.Length - 1] == '?' )
                        sb.AppendFormat("{0}={1}", kvp.Key, OAuth.Impl.OAuthBase.UrlEncode(kvp.Value));
                    else
                        sb.AppendFormat("&{0}={1}", kvp.Key, OAuth.Impl.OAuthBase.UrlEncode(kvp.Value));
                }
            }                        

            return GenerateSignedUrl(sb.ToString(), method, context, token);
        }

        static private String GetSignatureTypeString( OAuthBase.SignatureTypes type )
        {
            switch(type)
            {
                case OAuthBase.SignatureTypes.HMACSHA1:
                    return "HMACSHA1";
                case OAuthBase.SignatureTypes.PLAINTEXT:
                    return "PLAINTEXT";
                case OAuthBase.SignatureTypes.RSASHA1:
                    return "RSASHA1";
                default:
                    return "";
            }
        }
    
    
    }
}
