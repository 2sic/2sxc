namespace ToSic.Sxc.Tests.LinksAndImages;


public class UrlPartsTrivial: UrlPartsTestBase
{

    [Fact]
    public void TrivialUrls()
    {
        VerifyUrlOnly("https://www.xyz.org/something.jpg");
        VerifyUrlOnly("test");
        VerifyUrlOnly("test.jpg");
        VerifyUrlOnly("test.x.y.z.jpg");
    }
        

    private void VerifyUrlOnly(string url)
    {
        var urlParts = UrlParts(url);
        Equal(url, urlParts.Url);
        Equal(url, urlParts.ToLink());
        Equal(string.Empty, urlParts.Query);
        Equal(string.Empty, urlParts.Fragment);
    }

    [Fact]
    public void UrlsWithFragments()
    {
        VerifyUrlAndFragmentOnly("test#17", "test", "17");
        VerifyUrlAndFragmentOnly("test.jpg#abc=def", "test.jpg", "abc=def");
        VerifyUrlAndFragmentOnly("/test.jpg#", "/test.jpg", "");
        VerifyUrlAndFragmentOnly("test#first=1&message=hello?", "test", "first=1&message=hello?");
    }

    private void VerifyUrlAndFragmentOnly(string url, string pathExp, string fragmentExp)
    {
        var urlParts = UrlParts(url);
        Equal(url, urlParts.Url);
        Equal(pathExp, urlParts.Path);
        Equal(string.Empty, urlParts.Query);
        Equal(fragmentExp, urlParts.Fragment);
    }

}