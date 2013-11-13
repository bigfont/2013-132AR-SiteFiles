using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

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
            String enc = HttpUtility.UrlEncode(text, Encoding.UTF8);

            // fix the missing space
            enc = enc.Replace("+","%20");            

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
                    enc2.Append(Char.ToUpper(enc[i+2]));

                    i+=2;
                }                
            }

            return enc2.ToString();
        }
    }
}
