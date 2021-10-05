using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.WebTests
{
    [TestClass]
    public class UrlPartsTest
    {
        private UrlParts UrlParts(string url) => new UrlParts(url);

        [TestMethod]
        public void TrivialUrls()
        {
            VerifyUrlOnly("https://www.xyz.org/something.jpg");
            VerifyUrlOnly("test");
            VerifyUrlOnly("test.jpg");
            VerifyUrlOnly("test.x.y.z.jpg");
        }

        [TestMethod]
        public void ProtocolDetection()
        {
            void VerifyProtocol(string url, string protocol, string domain)
            {
                var urlParts = UrlParts(url);
                Assert.AreEqual(protocol, urlParts.Protocol);
                Assert.AreEqual(domain, urlParts.Domain);
                Assert.AreEqual(url ?? "", urlParts.ToLink(), "round trip should be identical except for null"); 
            }
            VerifyProtocol(null, "", "");
            VerifyProtocol("", "", "");
            VerifyProtocol("https:", "", "");
            VerifyProtocol("//abc.com", "//", "abc.com");
            VerifyProtocol("///abc.com", "//", "");
            VerifyProtocol("ftp://daniel@abc.com/q", "ftp://", "daniel@abc.com");
            VerifyProtocol("http://xyz.com", "http://", "xyz.com");
            VerifyProtocol("http//", "", "");
            VerifyProtocol("https://", "https://", "");
            VerifyProtocol("futureprot://", "futureprot://", "");
            VerifyProtocol("https://abc.com/", "https://", "abc.com");
            VerifyProtocol("http://abc/home", "http://", "abc");
            VerifyProtocol("http/test//", "", "");
            VerifyProtocol("../../test", "", "");
            VerifyProtocol("/test/abc?test", "", "");
        }

        [TestMethod]
        public void PostAddDomain()
        {
            void VerifyPostAddDomain(string expected, string url, string post)
            {
                var urlParts = UrlParts(url);
                urlParts.ReplaceDomain(post);
                Assert.AreEqual(expected, urlParts.ToLink());
            }
            VerifyPostAddDomain("//abc.org", "", "//abc.org");
            VerifyPostAddDomain("//abc.org", "", "abc.org");
            VerifyPostAddDomain("https://abc.org/", "https://xyz/", "abc.org");
            VerifyPostAddDomain("//abc.org/", "https://xyz/", "//abc.org");
        }

        private void VerifyUrlOnly(string url)
        {
            var urlParts = UrlParts(url);
            Assert.AreEqual(url, urlParts.Url);
            Assert.AreEqual(url, urlParts.ToLink());
            Assert.AreEqual(string.Empty, urlParts.Query);
            Assert.AreEqual(string.Empty, urlParts.Fragment);
        }

        [TestMethod]
        public void UrlsWithFragments()
        {
            VerifyUrlAndFragmentOnly("test#17", "test", "17");
            VerifyUrlAndFragmentOnly("test.jpg#abc=def", "test.jpg", "abc=def");
            VerifyUrlAndFragmentOnly("/test.jpg#", "/test.jpg", "");
            VerifyUrlAndFragmentOnly("test#first=1&message=hello?", "test", "first=1&message=hello?");
        }

        private void VerifyUrlAndFragmentOnly(string url, string pathExp, string fragmentExp)
        {
            var urlParts = UrlParts(url);
            Assert.AreEqual(url, urlParts.Url);
            Assert.AreEqual(pathExp, urlParts.Path);
            Assert.AreEqual(string.Empty, urlParts.Query);
            Assert.AreEqual(fragmentExp, urlParts.Fragment);
        }

    }
}
