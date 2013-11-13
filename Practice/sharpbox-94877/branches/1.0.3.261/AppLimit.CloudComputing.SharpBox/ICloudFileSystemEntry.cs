using System;
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
        /// This attribute contains the size of a file or the count of childs
        /// of the associated directory
        /// </summary>
        long Length { get; }

        /// <summary>
        /// This attribute contains the modification date of the object
        /// </summary>
        DateTime Modified { get; }
        
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

        /// <summary>
        /// This method gives raw access to the properties of the specific
        /// protocol provider
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        String GetPropertyValue(String key);
    }
}
