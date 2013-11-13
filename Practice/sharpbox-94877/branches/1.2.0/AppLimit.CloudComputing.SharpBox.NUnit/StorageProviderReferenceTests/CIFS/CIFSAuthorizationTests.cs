using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{    
    [TestFixture]
    [Category("ProviderIsPartOfAutoBuildTests")]
    public class LocalFSReferenceTests : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // build the credentials for dropbox
            StorageProvider.GenericCurrentCredentials cred = new AppLimit.CloudComputing.SharpBox.StorageProvider.GenericCurrentCredentials();

            StorageProvider.GenericNetworkCredentials icred = new AppLimit.CloudComputing.SharpBox.StorageProvider.GenericNetworkCredentials();
            icred.UserName = Environment.UserDomainName + "\\" + Environment.UserName;
            icred.Password = "MyWrongPassword";

            ValidAccessToken = cred;
            InvalidAccessToken = icred;
            Configuration = new AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS.CIFSConfiguration("C:\\IISUnitTestRoot");
            ConfigurationNonExistingServiceRoot = new AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS.CIFSConfiguration("C:\\IISUnitTestRoot\\" + Guid.NewGuid().ToString());
        }

        [Test]
        public void FolderDriveLetterIsRootTest()
        {
            CloudStorage cl = new CloudStorage();

            CIFSConfiguration NewConfiguration = new CIFSConfiguration("C:\\");

            cl.Open(NewConfiguration, ValidAccessToken);

            cl.GetRoot();

            cl.Close();

        }
    }
}
