using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    public class LinkHelperToTestBase: LinkHelperTestBase
    {
        private const string SkipTest = "!skip-test!";

        public void TestToPageParts(int? pageId = null, object p = null, string standard = SkipTest, string full = SkipTest, string protocol = SkipTest, string domain = SkipTest,
            string path = SkipTest, string file = SkipTest, string query = SkipTest, string hash = SkipTest, string suffix = SkipTest)
        {
            void TestPart(string expected, string part)
            {
                if (expected == SkipTest) return;
                // note: we do this on 2 lines of code to make debugging easier if the value is not what's expected
                var result = Link.TestTo(pageId: pageId, parameters: p, part: part);
                AreEqual(expected, result, $"Tested with part: {part}");
            }

            TestPart(standard, default);
            TestPart(full, "full");
            TestPart(protocol, "protocol");
            TestPart(domain, "domain");
            // not yet implemented features
            // TestPart(path, "path");
            // TestPart(file, "file");
            TestPart(query, "query");
            TestPart(hash, "hash");
            TestPart(suffix, "suffix");
        }
        
    }
}
