using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Api.Impl;
using Spring.Social.Dropbox.Connect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySpringSocialDropbox
{
    class Program
    {
        static void Main(string[] args)
        {            
            string consumerKey = "..."; // The application's consumer key
            string consumerSecret = "..."; // The application's consumer secret
            string accessToken = "..."; // The access token granted after OAuth authorization
            string accessTokenSecret = "..."; // The access token secret granted after OAuth authorization
            IDropbox dropbox = new DropboxTemplate(consumerKey, consumerSecret, accessToken, accessTokenSecret, AccessLevel.Full);


            DropboxServiceProvider serviceProvider = new DropboxServiceProvider("consumerKey", "consumerSecret", AccessLevel.AppFolder);
            OAuth1Operations oauthOperations = serviceProvider.AuthOperations;
            OAuthToken requestToken = oauthOperations.FetchRequestToken(null, null);
            OAuth1Parameters parameters = new OAuth1Parameters();
            parameters.CallbackUrl = "http://my-callback-url/";
            parameters.Add("locale", "fr-FR"); // for a localized version of the authorization website
            string authorizeUrl = oauthOperations.BuildAuthorizeUrl(requestToken, parameters);
            Response.Redirect(authorizeUrl);

            // upon receiving the callback from the provider:
            OAuthToken accessToken = oauthOperations.ExchangeForAccessToken(new AuthorizedRequestToken(requestToken, null), null);
            IDropbox dropboxApi = serviceProvider.GetApi(accessToken.Value, accessToken.Secret);



        }
    }
}
