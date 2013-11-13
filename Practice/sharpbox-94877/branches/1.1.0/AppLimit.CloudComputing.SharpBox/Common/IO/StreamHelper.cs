using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox.Common.IO
{       
    internal delegate void StreamHelperProgressCallback(long ReadByteTotal, long TotalLength, params Object[] data);

    internal class StreamHelper
    {
        private static int _BufferSize = 4096;

        public static void CopyStreamData(Stream src, Stream trg, StreamHelperProgressCallback status, params Object[] data)
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
            int readBytesTotal = 0;
            do
            {
                // Read the bytes
                readBytes = src.Read(buffer, 0, _BufferSize);

                // add
                readBytesTotal += readBytes;

                // Write the bytes
                if (readBytes > 0)
                {
                    trg.Write(buffer, 0, readBytes);

                    // notify state
                    if (status != null)
                    {
                        if (src.CanSeek)
                            status(readBytesTotal, src.Length, data);
                        else
                            status(readBytesTotal, -1, data);
                    }
                }
            } while (readBytes > 0);            
        }

        public static MemoryStream ToStream(String data)
        {
            // create the memory stream
            MemoryStream mStream = new MemoryStream();

            // write the data into
            StreamWriter sw = new StreamWriter(mStream);
            sw.Write(data);
            sw.Flush();

            // reset position
            mStream.Position = 0;

            // go ahead
            return mStream;
        }
    }
}
