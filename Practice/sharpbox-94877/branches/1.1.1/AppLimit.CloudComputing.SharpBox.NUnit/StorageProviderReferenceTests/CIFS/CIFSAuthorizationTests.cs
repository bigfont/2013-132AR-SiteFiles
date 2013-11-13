using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{    
    [TestFixture]
    public class LocalFSReferenceTests : GenericReferenceTests
    {
        public override void InitializeProvider()
        {
            // build the credentials for dropbox
            StorageProvider.GenericCurrentCredentials cred = new AppLimit.CloudComputing.SharpBox.StorageProvider.GenericCurrentCredentials();

            StorageProvider.GenericNetworkCredentials icred = new AppLimit.CloudComputing.SharpBox.StorageProvider.GenericNetworkCredentials();
            icred.UserName = Environment.UserDomainName + "\\" + Environment.UserName;
            icred.Password = "MyWrongPassword";
            
            Credentials = cred;
            InvalidCredentials = icred;
            Configuration = new AppLimit.CloudComputing.SharpBox.StorageProvider.CIFS.CIFSConfiguration("C:\\CIFSTestRoot");
        }

        [Test]
        public void FolderDriveLetterIsRootTest()
        {
            CloudStorage cl = new CloudStorage();

            CIFSConfiguration NewConfiguration = new CIFSConfiguration("C:\\");

            cl.Open(NewConfiguration, Credentials);

            cl.GetRoot();

            cl.Close();

        }
    }
}
