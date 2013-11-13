using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DropBox.CoreApi01;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Metadata m = MyProgram.GetMetadata();
            Assert.Equals(m.contents.Length, 0);
        }
    }
}
