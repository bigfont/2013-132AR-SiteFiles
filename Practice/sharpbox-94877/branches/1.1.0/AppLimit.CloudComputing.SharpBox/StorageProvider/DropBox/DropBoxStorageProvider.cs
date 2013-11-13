using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox.Logic;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox
{
    internal class DropBoxStorageProvider : GenericStorageProvider
    {
        public DropBoxStorageProvider()
            : base(new DropBoxStorageProviderService())
        { 
        }
    }
}
