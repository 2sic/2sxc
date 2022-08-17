using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.LinksAndImages
{
    [TestClass]
    public class UrlPartsTrivial: UrlPartsTestBase
    {

        [TestMethod]
        public void TrivialUrls()
        {
            VerifyUrlOnly("https://www.xyz.org/something.jpg");
            VerifyUrlOnly("test");
            VerifyUrlOnly("test.jpg");
            VerifyUrlOnly("test.x.y.z.jpg");
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
