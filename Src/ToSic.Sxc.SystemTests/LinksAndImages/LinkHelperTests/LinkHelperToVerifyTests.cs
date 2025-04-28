using ToSic.Sxc.Services;
using Xunit.Sdk;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperToVerifyTests(ILinkService link) : LinkHelperToTestBase(link)
{

    [Fact]
    //[ExpectedException(typeof(AssertFailedException))]
    public void TestTheTestStandard() => Throws<EqualException>(() => TestToPageParts(null, standard: "somethingwrong"));
        
}