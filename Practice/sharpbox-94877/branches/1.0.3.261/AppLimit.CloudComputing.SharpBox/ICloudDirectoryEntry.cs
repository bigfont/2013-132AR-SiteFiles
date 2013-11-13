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
        /// <returns>Reference to the file or folder</returns>
        ICloudFileSystemEntry GetChild(String name);        
    
        /// <summary>
        /// This method allows to get a directory entry with a specific index
        /// number
        /// </summary>
        /// <param name="idx">The index of the targeted folder</param>
        /// <returns>Reference to the file or folder</returns>
		ICloudFileSystemEntry GetChild(int idx);
		
        /// <summary>
        /// This property allows to access to the number of 
        /// child items
        /// </summary>
		int Count { get; }
	}
}
