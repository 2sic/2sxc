using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests
{
    [TestClass]
    public class CspPolicyTextProcessorTests
    {
        private readonly CspPolicyTextProcessor _processor = new(null);

        [TestMethod]
        public void EmptyWhenNull() => Assert.AreEqual(0, _processor.Parse(null).Count);

        [TestMethod]
        public void EmptyWhenEmpty() => Assert.AreEqual(0, _processor.Parse("").Count);

        [TestMethod]
        public void EmptyWhenSpaces() => Assert.AreEqual(0, _processor.Parse("   ").Count);

        [TestMethod]
        public void EmptyWhenEmptyLines() => Assert.AreEqual(0, _processor.Parse(" \n   \n  ").Count);

        [TestMethod]
        public void EmptyWhenCommentOnly() => Assert.AreEqual(0, _processor.Parse("// Comment").Count);

        [TestMethod]
        public void EmptyWhenMultipleCommentOnly() => Assert.AreEqual(0, _processor.Parse("  // Comment \n// comment2").Count);


        [TestMethod]
        [DataRow("default-src 'self'")]
        [DataRow("default-src:'self'")]
        [DataRow("default-src: 'self'")]
        [DataRow("default-src:   'self'")]
        [DataRow("default-src: 'self'  ")]
        public void SimplePair(string singleString)
        {
            var result = _processor.Parse(singleString);
            Assert.AreEqual(1, result.Count);
            var first = result.First();
            Assert.AreEqual(CspConstants.DefaultSrcName, first.Key);
            Assert.AreEqual("'self'", first.Value);
        }

    }
}
