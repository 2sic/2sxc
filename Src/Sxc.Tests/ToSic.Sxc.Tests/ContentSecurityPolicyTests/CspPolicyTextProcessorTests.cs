using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests;

[TestClass]
public class CspPolicyTextProcessorTests
{
    private readonly CspPolicyTextProcessor _processor = new(null);

    [TestMethod]
    public void EmptyWhenNull() => AreEqual(0, _processor.Parse(null).Count);

    [TestMethod]
    public void EmptyWhenEmpty() => AreEqual(0, _processor.Parse("").Count);

    [TestMethod]
    public void EmptyWhenSpaces() => AreEqual(0, _processor.Parse("   ").Count);

    [TestMethod]
    public void EmptyWhenEmptyLines() => AreEqual(0, _processor.Parse(" \n   \n  ").Count);

    [TestMethod]
    public void EmptyWhenCommentOnly() => AreEqual(0, _processor.Parse("// Comment").Count);

    [TestMethod]
    public void EmptyWhenMultipleCommentOnly() => AreEqual(0, _processor.Parse("  // Comment \n// comment2").Count);


    [TestMethod]
    [DataRow("default-src 'self'")]
    [DataRow("default-src:'self'")]
    [DataRow("default-src: 'self'")]
    [DataRow("default-src:   'self'")]
    [DataRow("default-src: 'self'  ")]
    public void SimplePair(string singleString)
    {
        var result = _processor.Parse(singleString);
        AreEqual(1, result.Count);
        var first = result.First();
        AreEqual(CspConstants.DefaultSrcName, first.Key);
        AreEqual("'self'", first.Value);
    }

}