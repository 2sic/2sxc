using ToSic.Sxc.Services;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;


public class LinkHelperToTestBase(ILinkService Link)
{
    private const string SkipTest = "!skip-test!";

    public void TestToPageParts(int? pageId = null, object? p = null, string standard = SkipTest, string full = SkipTest, string relative = SkipTest)
    {
        void TestType(string expected, string type)
        {
            if (expected == SkipTest) return;
            // note: we do this on 2 lines of code to make debugging easier if the value is not what's expected
            var result = Link.TestTo(pageId: pageId, parameters: p, type: type);
            Equal(expected, result);//, $"Tested with type: {type}");
        }

        TestType(standard, default);
        TestType(full, "full");
        TestType(full.Replace("https://", "//"), "//");
        TestType(relative, "/");

    }

}