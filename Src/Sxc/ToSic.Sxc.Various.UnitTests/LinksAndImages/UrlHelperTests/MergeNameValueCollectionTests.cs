using System.Collections.Specialized;
using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests;


public class MergeNameValueCollectionTests
{
    /// <summary>
    /// Test accessor
    /// </summary>
    /// <returns></returns>
    private NameValueCollection ImportTest(NameValueCollection first, NameValueCollection second, bool replace = false) =>
        first.Merge(second, replace);

    private const string Same = "!same!";

    private void Test(string exp, string expReplace, string first, string second)
    {
        var nvc1 = UrlHelpers.ParseQueryString(first);
        var itemsIn1 = nvc1.Count;
        var nvc2 = UrlHelpers.ParseQueryString(second);
        var itemsIn2 = nvc2.Count;
        var merged = ImportTest(nvc1, nvc2);
        Equal(itemsIn1, nvc1.Count);//, "Import shouldn't change first"); 
        Equal(itemsIn2, nvc2.Count);//, "Import shouldn't change second");
        Equal(exp, UrlHelpers.NvcToString(merged));

        merged = ImportTest(nvc1, nvc2, true);
        if (expReplace == Same) expReplace = exp;
        Equal(itemsIn1, nvc1.Count);//, "Import shouldn't change first"); 
        Equal(itemsIn2, nvc2.Count);//, "Import shouldn't change second");
        Equal(expReplace, UrlHelpers.NvcToString(merged));

    }

    [Fact] public void BasicMerge() => Test("first=1&second=2", Same, "first=1", "second=2");
    [Fact] public void LongerMerge() => Test("first=1&a=b&second=2&x=z", Same, "first=1&a=b", "second=2&x=z");
    [Fact] public void EmptyBoth() => Test("", Same, "", "");
    [Fact] public void EmptyFirst() => Test("second=2&x=z",Same, "", "second=2&x=z");
    [Fact] public void EmptySecond() => Test("first=1&a=b", Same, "first=1&a=b", "");

    [Fact] public void FirstJustKey() => Test("first&second=2&x=z", Same, "first", "second=2&x=z");

    [Fact] public void IdenticalKeys() => Test(
        "first=a&identical=a&identical=b&second=2&x=z", "first=a&identical=b&second=2&x=z",
        "first=a&identical=a", "identical=b&second=2&x=z");
}