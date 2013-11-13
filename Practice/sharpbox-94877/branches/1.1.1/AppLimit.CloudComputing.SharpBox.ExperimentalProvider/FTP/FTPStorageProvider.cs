using System;
using System.Collections.Generic;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web.Dav;
using AppLimit.CloudComputing.SharpBox.StorageProvider.WebDav;
using System.Net;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using AppLimit.CloudComputing.SharpBox.StorageProvider.FTP.Logic;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.FTP
{
	public class FTPStorageProvider : GenericStorageProvider
	{
        public FTPStorageProvider()
            : base( new FTPStorageProviderService())
		{}		
    }
}

