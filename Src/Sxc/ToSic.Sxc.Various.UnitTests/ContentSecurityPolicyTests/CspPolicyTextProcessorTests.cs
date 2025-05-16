using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests;


public class CspPolicyTextProcessorTests
{
    private readonly CspPolicyTextProcessor _processor = new(null);

    [Fact]
    public void EmptyWhenNull() => Equal(0, _processor.Parse(null).Count);

    [Fact]
    public void EmptyWhenEmpty() => Equal(0, _processor.Parse("").Count);

    [Fact]
    public void EmptyWhenSpaces() => Equal(0, _processor.Parse("   ").Count);

    [Fact]
    public void EmptyWhenEmptyLines() => Equal(0, _processor.Parse(" \n   \n  ").Count);

    [Fact]
    public void EmptyWhenCommentOnly() => Equal(0, _processor.Parse("// Comment").Count);

    [Fact]
    public void EmptyWhenMultipleCommentOnly() => Equal(0, _processor.Parse("  // Comment \n// comment2").Count);


    [Theory]
    [InlineData("default-src 'self'")]
    [InlineData("default-src:'self'")]
    [InlineData("default-src: 'self'")]
    [InlineData("default-src:   'self'")]
    [InlineData("default-src: 'self'  ")]
    public void SimplePair(string singleString)
    {
        var result = _processor.Parse(singleString);
        Equal(1, result.Count);
        var first = result.First();
        Equal(CspConstants.DefaultSrcName, first.Key);
        Equal("'self'", first.Value);
    }

}