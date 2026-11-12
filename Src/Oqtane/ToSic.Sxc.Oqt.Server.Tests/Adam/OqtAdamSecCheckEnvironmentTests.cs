using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Adam;

public class OqtAdamSecCheckEnvironmentTests
{
    private const string SiteList = "jpg, PNG,webm";

    [Theory]
    [InlineData("photo.jpg", true, "listed")]
    [InlineData("photo.JPG", true, "file extension in other case")]
    [InlineData("photo.png", true, "list entry in other case, with space")]
    [InlineData("video.webm", true, "listed, not in Oqtane default")]
    [InlineData("document.pdf", false, "in Oqtane default, but not in site list")]
    [InlineData("archive.jpg.exe", false, "only the last extension counts")]
    [InlineData("file", false, "no extension")]
    [InlineData("file.", false, "empty extension")]
    public void IsUploadableExtension_UsesSiteList_WhenSiteHasList(string fileName, bool expected, string _)
        => Equal(expected, OqtAdamSecCheckEnvironment.IsUploadableExtension(SiteList, fileName));

    [Theory]
    [InlineData(null, "document.pdf", true)]
    [InlineData("", "document.pdf", true)]
    [InlineData(null, "video.webm", false)]
    [InlineData("", "file", false)]
    public void IsUploadableExtension_UsesOqtaneDefault_WhenSiteHasNoList(string? siteList, string fileName, bool expected)
        => Equal(expected, OqtAdamSecCheckEnvironment.IsUploadableExtension(siteList, fileName));
}
