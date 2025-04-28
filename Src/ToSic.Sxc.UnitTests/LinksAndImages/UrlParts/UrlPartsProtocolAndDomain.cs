namespace ToSic.Sxc.Tests.LinksAndImages;


public class UrlPartsProtocolAndDomain : UrlPartsTestBase
{
        

    [Fact]
    public void ProtocolDetection()
    {
        void VerifyProtocol(string url, string protocol, string domain)
        {
            var urlParts = UrlParts(url);
            Equal(protocol, urlParts.Protocol);
            Equal(domain, urlParts.Domain);
            Equal(url ?? "", urlParts.ToLink());//, "round trip should be identical except for null"); 
        }
        VerifyProtocol(null, "", "");
        VerifyProtocol("", "", "");
        VerifyProtocol("https:", "", "");
        VerifyProtocol("//abc.com", "//", "abc.com");
        VerifyProtocol("///abc.com", "//", "");
        VerifyProtocol("ftp://daniel@abc.com/q", "ftp://", "daniel@abc.com");
        VerifyProtocol("http://xyz.com", "http://", "xyz.com");
        VerifyProtocol("http//", "", "");
        VerifyProtocol("https://", "https://", "");
        VerifyProtocol("futureprot://", "futureprot://", "");
        VerifyProtocol("https://abc.com/", "https://", "abc.com");
        VerifyProtocol("http://abc/home", "http://", "abc");
        VerifyProtocol("http/test//", "", "");
        VerifyProtocol("../../test", "", "");
        VerifyProtocol("/test/abc?test", "", "");
    }

    /// <summary>
    /// Verify Root Replacement
    /// </summary>
    private void VerRepRoot(string expected, string url, string post)
    {
        var urlParts = UrlParts(url);
        urlParts.ReplaceRoot(post);
        Equal(expected, urlParts.ToLink());
    }

    [Fact] public void RepRootEmptyOrigUsesReplacement() => VerRepRoot("//abc.org", "", "//abc.org");
    [Fact] public void RepRootOnlyDomainWorks() => VerRepRoot("//abc.org", "", "abc.org");
    [Fact] public void RepRootPreserveOrigProtocolIfNotNew() => VerRepRoot("https://abc.org/", "https://xyz/", "abc.org");
    [Fact] public void RepRootTakeNewProtocolIfGiven() => VerRepRoot("//abc.org/", "https://xyz/", "//abc.org");
    [Fact] public void RepRootSkipInvalidProtocol() => VerRepRoot("https://xyz/", "https://xyz/", "ftp:");
    [Fact] public void RepRootUseStandaloneProtocol() => VerRepRoot("ftp://xyz/", "https://xyz/", "ftp://");

    // TODO
    // VARIOUS combinations of ToLink



}