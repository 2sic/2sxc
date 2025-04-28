namespace ToSic.Sxc.Tests.LinksAndImages;


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
        Equal(expected, result);
    }

    [Fact] public void DomainAndProtocolToDefault() => To("https://abc.org", "https://abc.org", null);
    [Fact] public void DomainAndProtocolToFull() => To("https://abc.org", "https://abc.org", Full);
    [Fact] public void DomainAndProtocolToRel() => To("", "https://abc.org", Path);
    [Fact] public void DomainAndProtocolToRoot() => To("//abc.org", "https://abc.org", Root);

    [Fact] public void EmptyToDefault() => To("", "", null);
    [Fact] public void EmptyToFull() => To("", "", Full);

    [Fact] public void FullToDefault() => To(FullLink, FullLink, null);
    [Fact] public void FullToFull() => To(FullLink, FullLink, Full);
    [Fact] public void FullToBase() => To(FullPath, FullLink, Path);
    [Fact] public void FullToRoot() => To(FullRoot, FullLink, Root);

    [Fact] public void PathToDefault() => To(FullPath, FullPath, null);
    [Fact] public void PathToFull() => To(FullPath, FullPath, Full);
    [Fact] public void PathToBase() => To(FullPath, FullPath, Path);
    [Fact] public void PathToRoot() => To(FullPath, FullPath, Root);

}