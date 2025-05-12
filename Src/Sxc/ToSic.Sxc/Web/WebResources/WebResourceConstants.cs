namespace ToSic.Sxc.Web.WebResources;

internal class WebResourceConstants
{
    public static string WebResourcesNode = "WebResources";

    public static string CdnSourcePublicField = "CdnSourcePublic";

    public static string CdnSourceEditField = "CdnSourceEdit";

    // Note: this should not change often
    // As usually new versions of assets will often run side-by side with older versions
    // But do keep in sync w/2sxc versions (possibly just not upgrade as only small things change) for clarity
    public const string VersionSuffix = "/v15";

    //internal const string Cdn2SxcRoot = "https://cdn.2sxc.org/packages";
    //internal const string CdnLocalRoot = "/cdn/packages";

    internal const string CdnDefault = "cdn";
    //internal const string Cdn2Sxc = "cdn.2sxc.org";
    //internal const string CdnLocal = "local";
}