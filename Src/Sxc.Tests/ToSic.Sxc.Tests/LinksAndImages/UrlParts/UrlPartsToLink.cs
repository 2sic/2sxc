using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages
{
    [TestClass]
    public class UrlPartsToLink : UrlPartsTestBase
    {
        // TODO
        // VARIOUS combinations of ToLink
        // todo
        // different casing of full
        // unknown types
        // "absolute" as alternative to full ?
        // diff initial setups


        private const string Full = "full";
        private const string Path = "/";
        private const string Root = "//";

        private const string FullLink = "https://abcd.com/path/name?param=27#ok=true";
        private const string FullPath = "/path/name?param=27#ok=true";
        private const string FullRoot = "//abcd.com" + FullPath;

        private void To(string expected, string url, string type)
        {
            var result = UrlParts(url).ToLink(type);
            AreEqual(expected, result);
        }

        [TestMethod] public void DomainAndProtocolToDefault() => To("https://abc.org", "https://abc.org", null);
        [TestMethod] public void DomainAndProtocolToFull() => To("https://abc.org", "https://abc.org", Full);
        [TestMethod] public void DomainAndProtocolToRel() => To("", "https://abc.org", Path);
        [TestMethod] public void DomainAndProtocolToRoot() => To("//abc.org", "https://abc.org", Root);

        [TestMethod] public void EmptyToDefault() => To("", "", null);
        [TestMethod] public void EmptyToFull() => To("", "", Full);

        [TestMethod] public void FullToDefault() => To(FullLink, FullLink, null);
        [TestMethod] public void FullToFull() => To(FullLink, FullLink, Full);
        [TestMethod] public void FullToBase() => To(FullPath, FullLink, Path);
        [TestMethod] public void FullToRoot() => To(FullRoot, FullLink, Root);

        [TestMethod] public void PathToDefault() => To(FullPath, FullPath, null);
        [TestMethod] public void PathToFull() => To(FullPath, FullPath, Full);
        [TestMethod] public void PathToBase() => To(FullPath, FullPath, Path);
        [TestMethod] public void PathToRoot() => To(FullPath, FullPath, Root);

    }
}
