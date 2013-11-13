using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.CloudComputing.OAuth.Impl;

namespace AppLimit.CloudComputing.OAuth.Context
{
    public class OAuthConsumerContext
    {
        public readonly String ConsumerKey;
        public readonly String ConsumerSecret;
        public readonly SignatureTypes SignatureMethod = SignatureTypes.HMACSHA1;

        public OAuthConsumerContext(String ConsumerKey, String ConsumerSecret)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
        }
    }
}
