using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AppLimit.CloudComputing.SharpBox.OAuth.Impl;

namespace AppLimit.CloudComputing.SharpBox.OAuth.Context
{
    internal class OAuthConsumerContext
    {
        public readonly String ConsumerKey;
        public readonly String ConsumerSecret;
        public readonly OAuthBase.SignatureTypes SignatureMethod = OAuthBase.SignatureTypes.HMACSHA1;

        public OAuthConsumerContext(String ConsumerKey, String ConsumerSecret)
        {
            this.ConsumerKey = ConsumerKey;
            this.ConsumerSecret = ConsumerSecret;
        }
    }
}
