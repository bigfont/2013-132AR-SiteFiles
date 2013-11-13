using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.Common
{
    internal class StreamHelper
    {
        private static int _BufferSize = 4096;

        public static void CopyStreamData(Stream src, Stream trg)
        {
            // validate parameter
            if (src == null || trg == null)
                return;

            if (src.CanRead == false || trg.CanWrite == false)
                return;

            // build the buffer as configured
            byte[] buffer = new byte[_BufferSize];

            // copy the stream data
            int readBytes = 0;

            do
            {
                readBytes = src.Read(buffer, 0, _BufferSize);
                if (readBytes > 0)
                    trg.Write(buffer, 0, readBytes);

            } while (readBytes > 0);            
        }
    }
}
