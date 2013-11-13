using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.API
{
    interface IStorageProviderSession
    {
        /// <summary>
        /// The generated access token for this 
        /// session
        /// </summary>
        ICloudStorageAccessToken SessionToken { get; }

        /// <summary>
        /// The associated service which is connected
        /// with this session
        /// </summary>
        IStorageProviderService Service { get; }

        /// <summary>
        /// A valid cloud storage service configuration
        /// </summary>
        ICloudStorageConfiguration ServiceConfiguration { get; }
    }
}
