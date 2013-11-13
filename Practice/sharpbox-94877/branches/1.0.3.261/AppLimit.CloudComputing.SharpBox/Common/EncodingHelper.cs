using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT || ANDROID
using System.Net;
#else
using System.Web;
#endif

namespace AppLimit.CloudComputing.SharpBox.Common
{
    internal class EncodingHelper
    {
        public static string UTF8Encode(string text)
        {
            if (text == null)
                return null;

            if (text.Length == 0)
                return "";

            // encode with url encoder
#if SILVERLIGHT || ANDROID
            String enc = HttpUtility.UrlEncode(text);
#else
            String enc = HttpUtility.UrlEncode(text, Encoding.UTF8);
#endif

            // fix the missing space
            enc = enc.Replace("+", "%20");

            // fix the exclamation point
            enc = enc.Replace("!", "%21");

            // fix the quote
            enc = enc.Replace("'", "%27");

            // fix the parentheses
            enc = enc.Replace("(", "%28");
            enc = enc.Replace(")", "%29");

            // uppercase the encoded stuff            
            StringBuilder enc2 = new StringBuilder();

            for (int i = 0; i < enc.Length; i++)
            {
                // copy char
                enc2.Append(enc[i]);

                // upper stuff
                if (enc[i] == '%')
                {

                    enc2.Append(Char.ToUpper(enc[i + 1]));
                    enc2.Append(Char.ToUpper(enc[i + 2]));

                    i += 2;
                }
            }

            return enc2.ToString();
        }
    }
}
