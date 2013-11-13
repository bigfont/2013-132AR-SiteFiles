using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AppLimit.CloudComputing.OAuth.Impl
{
    class OAuthMultipartFormDataSupport
    {
        private const string FormBoundary = "-----------------------------28947758029299";

        public string GetMultipartFormContentType()
        {
            return string.Format("multipart/form-data; boundary={0}", FormBoundary);
        }

        public void WriteMultipartFormData(Stream webRequestStream, Dictionary<String, Byte[]> dataCollection)
        {
            var encoding = Encoding.ASCII;

            foreach (KeyValuePair<String, Byte[]> kvp in dataCollection)
            {
                var length = kvp.Value.Length;
                
                // Add just the first part of this param, since we will write the file data directly to the Stream
                string header = string.Format("--{0}{4}Content-Disposition: form-data; name=\"{2}\"; filename=\"{1}\";{4}Content-Type: {3}{4}{4}",
												FormBoundary,
                                                kvp.Key,
                                                "file",
                                                "application/octet-stream",
                                                Environment.NewLine);

                webRequestStream.Write(encoding.GetBytes(header), 0, header.Length);
                // Write the file data directly to the Stream, rather than serializing it to a string.
                webRequestStream.Write(kvp.Value, 0, (int)length);

				string footer = String.Format("{1}--{0}--{1}", FormBoundary, Environment.NewLine);
                webRequestStream.Write(encoding.GetBytes(footer), 0, footer.Length);
            }
        }        
    }
}
