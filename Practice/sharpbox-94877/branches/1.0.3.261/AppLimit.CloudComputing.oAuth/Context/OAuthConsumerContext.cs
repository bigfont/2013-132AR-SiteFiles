using System;

using AppLimit.CloudComputing.OAuth.Impl;

namespace AppLimit.CloudComputing.OAuth.Context
{
    public class OAuthConsumerContext
    {
        public readonly String ConsumerKey;
        public readonly String ConsumerSecret;
        public readonly SignatureTypes SignatureMethod = SignatureTypes.HMACSHA1;

        public OAuthConsumerContext(String consumerKey, String consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }
    }
}
