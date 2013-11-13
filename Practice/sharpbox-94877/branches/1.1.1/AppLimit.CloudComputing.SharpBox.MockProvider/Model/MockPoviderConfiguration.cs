using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox.MockProvider.Model
{
    public class MockPoviderConfiguration : ICloudStorageConfiguration
    {
        #region ICloudStorageConfiguration Members

        public Uri ServiceLocator
        {
            get { return new Uri("mock://localhost"); }
        }

        public bool TrustUnsecureSSLConnections
        {
            get { return false; }
        }

        public CloudStorageLimits Limits
        {
            get { return new CloudStorageLimits(); }
        }

        #endregion
    }
}
