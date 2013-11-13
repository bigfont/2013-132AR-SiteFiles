using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace AppLimit.CloudComputing.SharpBox
{
    /// <summary>
    /// This enum describes the data transfer direction
    /// </summary>
    public enum nTransferDirection
    {
        /// <summary>
        /// Defines that the target data stream should be uploaded into the cloud file container
        /// </summary>
        nUpload,

        /// <summary>
        /// Defines that the data from the cloud file container should be downloaded into the target data stream
        /// </summary>
        nDownload
    };

    /// <summary>
    /// This delegate can be used as callback for upload or download operation in the 
    /// data streams.
    /// </summary>
    /// <param name="file">Reference to the file in the cloud storage</param>
    /// <param name="currentbytes">amount of bytes which are up/downloaded currently</param>
    /// <param name="sizebytes">total size of bytes for this file</param>
    /// <param name="progressContext">context for progress</param>
    public delegate void FileOperationProgressChanged(ICloudFileSystemEntry file, long currentbytes, long sizebytes, Object progressContext);

    /// <summary>
    /// This interface implements a specifc transfer logic which can be used
    /// to transport data from a local data stream to a remote filesystem entry 
    /// and back
    /// </summary>
    public interface ICloudFileDataTransfer
    {
        /// <summary>
        /// This method transfers data between a local data stream and the remote filesystem entry on
        /// byte level
        /// </summary>        
        /// <param name="targetDataStream"></param>
        /// <param name="direction"></param>
        /// <param name="progressCallback"></param>
        /// <param name="progressContext"></param>        
        void Transfer(Stream targetDataStream, nTransferDirection direction, FileOperationProgressChanged progressCallback, Object progressContext);

        /// <summary>
        /// Allows native access to the download stream of the associated file. 
        /// Ensure that this stream will be disposed clearly!
        /// </summary>
        Stream GetDownloadStream();

        /// <summary>
        /// Allows native access to the upload stream of the associated file
        /// Ensure that this stream will be disposed clearly!
        /// </summary>
        /// <param name="uploadSize"></param>        
        Stream GetUploadStream(long uploadSize);
        
#if !WINDOWS_PHONE
        /// <summary>
        /// This method supports the serialization of object graphs into the remote file container
        /// </summary>        
        /// <param name="dataFormatter"></param>
        /// <param name="objectGraph"></param>
        void Serialize(IFormatter dataFormatter, Object objectGraph);

        /// <summary>
        /// This method allows to deserialize an object graph from the remote file container
        /// </summary>        
        /// <param name="dataFormatter"></param>
        /// <returns></returns>
        Object Deserialize(IFormatter dataFormatter);
#endif

    }
}
