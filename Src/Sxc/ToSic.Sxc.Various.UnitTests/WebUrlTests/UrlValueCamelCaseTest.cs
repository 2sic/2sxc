using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Tests.WebUrlTests;


public class UrlValueCamelCaseTest
{
    private UrlValueCamelCase TestProcess() => new();

    [Theory]
    [InlineData("original", "original", "all lower case")]
    [InlineData("original", "Original", "Pascal to lower case")]
    [InlineData("originalThing", "OriginalThing", "Pascal to camel case")]
    [InlineData("oRIGINAL", "ORIGINAL", "Caps to weird case")]
    public void BasicTests(string expected, string name, string message)
        => Equal(expected, TestProcess().Process(new(name, null)).Name);

}