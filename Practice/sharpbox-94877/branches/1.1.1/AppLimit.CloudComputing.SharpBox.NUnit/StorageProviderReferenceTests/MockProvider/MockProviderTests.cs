using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.MockProvider.Model;


namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{
    [TestFixture]
    public class MockProviderTests : GenericReferenceTests
    {
        public override void InitializeProvider()
        {            
            // build the credentials for dropbox
            MockProviderCredentials cred = new MockProviderCredentials();
            MockProviderCredentials icred = new MockProviderCredentials();
            
            Credentials = cred;
            InvalidCredentials = icred;

            // set the configuration
            Configuration = new MockPoviderConfiguration();

            // register the exernal provider
            this.cloudStorage.RegisterStorageProvider(Configuration.GetType(), typeof(MockProvider.MockProvider));
        }       
    }
}
