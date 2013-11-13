using System;
using System.Collections.Generic;

namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// Normally file based storage systems are supporting a hierarchical
    /// folder structure. This interface will be used to get access on the 
    /// folders in the cloud storage 
    /// </summary>
    public interface ICloudDirectoryEntry : ICloudFileSystemEntry, IEnumerable<ICloudFileSystemEntry>
    {        
        /// <summary>
        /// This method allows to get a directory entry with a specific folder 
        /// name.
        /// </summary>
        /// <param name="name">The name of the targeted folder</param>
        /// <returns>Reference to the folder</returns>
        ICloudFileSystemEntry GetChild(String name);        
    }
}
