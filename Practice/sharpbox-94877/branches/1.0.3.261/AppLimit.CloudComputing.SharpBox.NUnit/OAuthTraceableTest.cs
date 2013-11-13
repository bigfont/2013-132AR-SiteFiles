using System;
using System.Diagnostics;
using System.IO;
using System.Net;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.Common.Toolkit.Helper;
using AppLimit.CloudComputing.OAuth;

namespace AppLimit.CloudComputing.SharpBox.NUnit
{
    public class OAuthTraceableTest
    {
        public int WebRequestCount;
        public int WebRequestFixture;
        public TimeSpan WebTimeInFixture;
        public TimeSpan WebTimeInTest;

        [TestFixtureSetUpAttribute]
        public void SetupFixture()
        {
            // reset
            WebRequestFixture = 0;
            WebTimeInFixture = new TimeSpan();

            // add the trace code
            OAuthServiceManager.Instance.requestExecutedEvent += Instance_requestExecutedEvent;
        }

        [TestFixtureTearDown()]
        public void TearDownFixture()
        {
            // remove trace code
            OAuthServiceManager.Instance.requestExecutedEvent -= Instance_requestExecutedEvent;

            // output
            Console.WriteLine("");
            Console.WriteLine("WebTime in fixture: " + WebTimeInFixture.ToString());
            Console.WriteLine("");
        }

        [SetUp]
        public void SetupTest()
        {
            // reset the request count
            WebRequestCount = 0;
            WebTimeInTest = new TimeSpan();
        }

        [TearDown]
        public void TearTest()
        {
            // output
            Console.WriteLine("");
            Console.WriteLine("WebTime in test: " + WebTimeInTest.ToString());
            Console.WriteLine("");
        }

        void Instance_requestExecutedEvent(Object sender, WebRequest request, TimeSpan timeNeeded, HttpStatusCode statusCode, Stream resultStream)
        {
            WebRequestCount++;
            WebRequestFixture++;

            Console.WriteLine("WebRequest detected");
            Console.WriteLine("    Count: " + WebRequestCount + "/" + WebRequestFixture);
            Console.WriteLine("   Target: " + request.RequestUri.ToString());
            Console.WriteLine("     Took: " + timeNeeded.ToString());
            Console.WriteLine("HTTP-Code: " + statusCode.ToString());

            if (statusCode != HttpStatusCode.OK)
            {
                StreamReader rd = new StreamReader(resultStream);
                String value = rd.ReadToEnd();
                Console.WriteLine("Content: " + value);
            }

            WebTimeInFixture += timeNeeded;
            WebTimeInTest += timeNeeded;
        }
    }
}
