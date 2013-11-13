using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Exceptions;

namespace AppLimit.CloudComputing.SharpBox.NUnit.StorageProviderReferenceTests
{    
    public abstract partial class GenericReferenceTests : ReferenceBaseTest 
    {        
        /*[Test]
        public void ExceptionTestRessourceText()
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
        }*/
    }
}
