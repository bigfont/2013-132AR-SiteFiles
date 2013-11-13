using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// This class contains the limits of a given cloud storage
    /// configuration
    /// </summary>
    public class CloudStorageLimits
    {
        /// <summary>
        /// defines the maximum file size in bytes during upload
        /// </summary>
        public int MaxUploadFileSize { get; internal set; }

        /// <summary>
        /// defines the maximum file size in bytes during download
        /// </summary>
        public int MaxDownloadFileSize { get; internal set; }
    }
}
