using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AspDropBox.Core;
using AspDropBox.Core.Models;
using System.Linq;

namespace Dropbox.Core.Tests
{
    [TestClass]
    public class DropBoxCoreAPI
    {
        DropBoxCoreApi api;
        public DropBoxCoreAPI()
        {
            api = new DropBoxCoreApi("SxQS0Gl4VgQAAAAAAAAAAUKQZEZN088vu-NY0rA-vnALIsut1e0aFTSAK7oePPt8");
        }
        [TestMethod]
        public void GetMetadataFromFirstSubDir()
        {
            Metadata meta = api.GetMetadata(RootType.Sandbox);
            Metadata content = meta.Contents.First<Metadata>(c => c.Is_Dir);
            string name = content.Path;

            Metadata metaSub = api.GetMetadata(RootType.Sandbox, content.Path);
        }

        [TestMethod]
        public void GetMetadataFromRootFolder()
        {
            Metadata meta = api.GetMetadata(RootType.Sandbox);
            Assert.IsNotNull(meta);
            if(meta != null)
            { 
               Assert.IsNotNull(meta.Contents);
               if(meta.Contents != null)
               {
                   Assert.AreNotEqual(meta.Contents.Length, 0);
               }
            }
        }

        [TestMethod]
        public void GetAccountInfo()
        {
            AccountInfo info = api.GetAccountInfo();
            Assert.IsNotNull(info);
            if(info != null)
            {
                Assert.IsNotNull(info.Display_Name);
            }            
        }
    }
}
