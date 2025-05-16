using ToSic.Sxc.Services;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class LinkHelperToCommonTests(ILinkService Link)
{
    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void ToConflictingValuesProvidedTest()
    {
        Throws<ArgumentException>(() => Link.TestTo(pageId: 27, api: "api"));
    }
}