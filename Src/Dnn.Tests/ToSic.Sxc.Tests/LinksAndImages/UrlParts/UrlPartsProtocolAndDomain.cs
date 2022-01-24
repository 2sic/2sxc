using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.LinksAndImages
{
    [TestClass]
    public class UrlPartsProtocolAndDomain : UrlPartsTestBase
    {
        

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

        /// <summary>
        /// Verify Root Replacement
        /// </summary>
        private void VerRepRoot(string expected, string url, string post)
        {
            var urlParts = UrlParts(url);
            urlParts.ReplaceRoot(post);
            Assert.AreEqual(expected, urlParts.ToLink());
        }

        [TestMethod] public void RepRootEmptyOrigUsesReplacement() => VerRepRoot("//abc.org", "", "//abc.org");
        [TestMethod] public void RepRootOnlyDomainWorks() => VerRepRoot("//abc.org", "", "abc.org");
        [TestMethod] public void RepRootPreserveOrigProtocolIfNotNew() => VerRepRoot("https://abc.org/", "https://xyz/", "abc.org");
        [TestMethod] public void RepRootTakeNewProtocolIfGiven() => VerRepRoot("//abc.org/", "https://xyz/", "//abc.org");
        [TestMethod] public void RepRootSkipInvalidProtocol() => VerRepRoot("https://xyz/", "https://xyz/", "ftp:");
        [TestMethod] public void RepRootUseStandaloneProtocol() => VerRepRoot("ftp://xyz/", "https://xyz/", "ftp://");

        // TODO
        // VARIOUS combinations of ToLink



    }
}
