using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;

namespace AppLimit.CloudComputing.SharpBox.IntegrationTests.UnitTestClasses
{
    public class SharpBoxTestItem
    {
        public CloudStorage CloudStorage { get; private set; }

        public SharpBoxTestItem()
        {
            CloudStorage = new CloudStorage();
        }                                    
    }
}
