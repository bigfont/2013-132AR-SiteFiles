using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using AppLimit.CloudComputing.OAuth.Context;
using AppLimit.CloudComputing.OAuth.Token;

namespace AppLimit.CloudComputing.OAuth.Impl
{
    public class OAuthUrlGenerator
    {
        static public String GenerateRequestTokenUrl(String baseRequestTokenUrl, OAuthConsumerContext context)
        {
            return GenerateSignedUrl(baseRequestTokenUrl, "GET", context, null);            
        }

        static public String GenerateAuthorizationUrl(String baseAuthorizeUrl, String callbackUrl, OAuthToken requestToken)
        {
            var sb = new StringBuilder(baseAuthorizeUrl);
            
            sb.AppendFormat("?oauth_token={0}", HttpUtility.UrlEncode(requestToken.TokenKey));
            sb.AppendFormat("&oauth_callback={0}", HttpUtility.UrlEncode(callbackUrl));

            return sb.ToString();
        }

        static public String GenerateAccessTokenUrl(String baseAccessTokenUrl, OAuthConsumerContext context, OAuthToken requestToken)
        {
            return GenerateSignedUrl(baseAccessTokenUrl, "GET", context, requestToken);            
        }

        static private String GenerateSignedUrl(String baseRequestTokenUrl, String method, OAuthConsumerContext context, OAuthToken token)
        {
            String normalizedUrl;
            String normalizedRequestParameters;

            var oAuth = new OAuthBase();

            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string sig = oAuth.GenerateSignature(   new Uri(baseRequestTokenUrl),
                                                    context.ConsumerKey, context.ConsumerSecret,
                                                    token == null ? string.Empty : token.TokenKey, token == null ? string.Empty : token.TokenSecret,
                                                    method, timeStamp, nonce,
                                                    context.SignatureMethod,
                                                    out normalizedUrl, out normalizedRequestParameters);

            sig = HttpUtility.UrlEncode(sig);

            var sb = new StringBuilder(normalizedUrl);

            sb.AppendFormat("?");
            sb.AppendFormat(normalizedRequestParameters);
            sb.AppendFormat("&oauth_signature={0}", sig);

            return sb.ToString();
        }

        static public String GenerateSignedUrl(String baseRessourceUrl, String method, OAuthConsumerContext context, OAuthToken token, Dictionary<String, String> urlParameter)
        {            
            var sb = new StringBuilder(baseRessourceUrl);

            if (urlParameter != null)
            {
                sb.Append('?');

                foreach (KeyValuePair<String, String> kvp in urlParameter)
                {   
                    if ( sb[sb.Length - 1] == '?' )
                        sb.AppendFormat("{0}={1}", kvp.Key, OAuthBase.UrlEncode(kvp.Value));
                    else
                        sb.AppendFormat("&{0}={1}", kvp.Key, OAuthBase.UrlEncode(kvp.Value));
                }
            }                        

            return GenerateSignedUrl(sb.ToString(), method, context, token);
        }

        static private String GetSignatureTypeString( SignatureTypes type )
        {
            switch(type)
            {
                case SignatureTypes.HMACSHA1:
                    return "HMACSHA1";
                case SignatureTypes.PLAINTEXT:
                    return "PLAINTEXT";
                case SignatureTypes.RSASHA1:
                    return "RSASHA1";
                default:
                    return "";
            }
        }
    }
}
