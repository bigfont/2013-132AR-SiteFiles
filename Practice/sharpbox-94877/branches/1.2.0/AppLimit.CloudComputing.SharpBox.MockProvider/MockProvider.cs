using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider;
using AppLimit.CloudComputing.SharpBox.MockProvider.Logic;

namespace AppLimit.CloudComputing.SharpBox.MockProvider
{
    /// <summary>
    /// The MockProvider as self is just a connection between the specific
    /// service implementation and the GenericStorageProvider delivered
    /// by SharpBox out of the Box.
    /// </summary>
    public class MockProvider : GenericStorageProvider
    {
        public MockProvider()
            : base(new MockProviderService())
        { }
    }
}
