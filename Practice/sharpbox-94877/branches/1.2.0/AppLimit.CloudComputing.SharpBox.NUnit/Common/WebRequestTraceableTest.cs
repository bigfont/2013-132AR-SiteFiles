using System;
using System.Diagnostics;
using System.IO;
using System.Net;

using NUnit.Framework;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web;
using AppLimit.CloudComputing.SharpBox.Common.Net;

namespace AppLimit.CloudComputing.SharpBox.NUnit.Common
{
    public class WebRequestTraceableTest
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
            WebRequestManager.Instance.RequestPreparedEvent += Instance_RequestPreparedEvent;
            WebRequestManager.Instance.RequestExecutingEvent += Instance_RequestExecutingEvent;
            WebRequestManager.Instance.RequestExecutedEvent += Instance_requestExecutedEvent;
        }            

        [TestFixtureTearDown()]
        public void TearDownFixture()
        {
            // remove trace code
            WebRequestManager.Instance.RequestExecutingEvent -= Instance_RequestPreparedEvent;
            WebRequestManager.Instance.RequestExecutedEvent -= Instance_requestExecutedEvent;

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

        void Instance_RequestPreparedEvent(object sender, WebRequestExecutingEventArgs e)
        {
            WebRequestCount++;
            WebRequestFixture++;

            Console.WriteLine("WebRequest detected");
            Console.WriteLine("     Count: " + WebRequestCount + "/" + WebRequestFixture);
            Console.WriteLine("    Target: " + e.request.RequestUri.ToString());
            Console.WriteLine("    Method: " + e.request.Method);

            if (e.request is HttpWebRequest)
            {
                Console.WriteLine("  Buffered: " + (((HttpWebRequest)e.request).AllowWriteStreamBuffering ? "true" : "false"));
                Console.WriteLine("   Chunked: " + (((HttpWebRequest)e.request).SendChunked ? "true" : "false"));
                Console.WriteLine("   PreAuth: " + (((HttpWebRequest)e.request).PreAuthenticate ? "true" : "false"));
            }
            Console.WriteLine("   Timeout: " + e.request.Timeout.ToString() + " sec");

            if (e.request is HttpWebRequest)
            {
                Console.WriteLine(" RWTimeout: " + ((HttpWebRequest)e.request).ReadWriteTimeout.ToString() + " sec");
            }
        }

        void Instance_RequestExecutingEvent(object sender, WebRequestExecutingEventArgs e)
        {
            Console.WriteLine("    Length: " + e.request.ContentLength);
            DumpWebHeader(e.request.Headers);
        }

        void Instance_requestExecutedEvent(Object sender, WebRequestExecutedEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("WebResponnse received:");

            Console.WriteLine("      Took: " + e.timeNeeded.ToString());

            if (e.response != null && e.response is HttpWebResponse)
            {
                Console.WriteLine("HTTP-Code: " + (e.response as HttpWebResponse).StatusCode.ToString());

                if (!HttpUtilityEx.IsSuccessCode((e.response as HttpWebResponse).StatusCode))
                {
                    StreamReader rd = new StreamReader(e.resultStream);
                    String value = rd.ReadToEnd();
                    Console.WriteLine("Content: " + (value.Length > 100 ? value.Substring(0, 99) : value));
                }                
            }
            else if (e.exception != null)
            {
                Console.WriteLine("WebStatus: " + e.exception.Status.ToString());

                if (e.exception.Response != null && e.exception.Response.Headers != null)
                    DumpWebHeader(e.exception.Response.Headers);
                
            }
            else if (e.response == null && e.exception == null)
            {
                Console.WriteLine("HTTP-Code: Bad or no response");
            }

            WebTimeInFixture += e.timeNeeded;
            WebTimeInTest += e.timeNeeded;
        }

        private static void DumpWebHeader(WebHeaderCollection hdc)
        {
            if (hdc.AllKeys.Length == 0)
            {
                Console.WriteLine("Headerdump: no special headers");
            }
            else
            {
                Console.WriteLine("Headerdump: ");
                foreach (String key in hdc.AllKeys)
                {
                    Console.WriteLine("    Header: " + key + " -> " + hdc[key]);
                }
            }
        }

    }
}
