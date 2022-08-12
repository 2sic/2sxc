using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Tests.WebUrlTests
{
    [TestClass]
    public class UrlValueCamelCaseTest
    {
        private UrlValueCamelCase TestProcess() => new UrlValueCamelCase();

        [TestMethod]
        [DataRow("original", "original", "all lower case")]
        [DataRow("original", "Original", "Pascal to lower case")]
        [DataRow("originalThing", "OriginalThing", "Pascal to camel case")]
        [DataRow("oRIGINAL", "ORIGINAL", "Caps to weird case")]
        public void BasicTests(string expected, string name, string message) 
            => Assert.AreEqual(expected, TestProcess().Process(new NameObjectSet(name, null)).Name, message);

    }
}
