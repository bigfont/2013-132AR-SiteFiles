using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
    [TestFixture()]
    public class SharpBoxErrorExceptionTest
    {
        [Test]
        public void TestRessourceText()
        {
            Assert.IsTrue(VerifyThrowedSharpBoxException(SharpBoxErrorCodes.ErrorCouldNotContactStorageService, "Couldn't contact storage service"));            
        }

        private static Boolean VerifyThrowedSharpBoxException(SharpBoxErrorCodes code, String MessageNeeded)
        {
            try
            {
                throw new SharpBoxException(code);
            }
            catch (SharpBoxException e)
            {
                // verify the text
                Assert.AreEqual(e.Message, MessageNeeded);

                // ok
                return true;
            }            
        }
    }
}
