using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// Normally file based storage systems are supporting file system
    /// entries. This interface will be used to get access on the 
    /// file system entry in the cloud storage 
    /// </summary>
    public interface ICloudFileSystemEntry
    {
        /// <summary>
        /// The name of the associated folder
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The parent folder of the associated folder, can be null if it's 
        /// the root folder
        /// </summary>
        ICloudDirectoryEntry Parent { get; set; }

        /// <summary>
        /// This method give access to the content of a file (in write or read mode)
        /// </summary>
        /// <param name="access">Read or Write</param>
        /// <returns></returns>
        Stream GetContentStream(FileAccess access);
    }
}
