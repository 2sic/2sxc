using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.DI;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Integration.Paths;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("for testing / un-implemented use")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class LinkServiceUnknown(ImgResizeLinker imgLinker, LazySvc<ILinkPaths> linkPathsLazy, WarnUseOfUnknown<LinkServiceUnknown> _)
    : LinkServiceBase(imgLinker, linkPathsLazy), IIsUnknown
{
    public const string DefDomain = "unknown.2sxc.org";
    public const string DefProtocol = "https";
    public const string DefRoot = DefProtocol +"://" + DefDomain;
    public const string NiceCurrentPath = "/folder/sub-folder/";
    public const string NiceCurrentPage = "current-page";
    public const string NiceCurrentRelative = NiceCurrentPath + NiceCurrentPage;

    public const string NiceCurrentUrlRoot = DefRoot + NiceCurrentRelative;
    public const string NiceCurrentUrl = NiceCurrentUrlRoot;

    public const string UglyAnyQuery = "tabId={0}";
    public const string UglyCurrentQuery = "tabId=27";
    public const string UglyCurrentPage = "default.aspx?" + UglyCurrentQuery;
    public const string UglyAnyPage = "default.aspx?" + UglyAnyQuery;
    public const string UglyCurrentUrl = DefRoot + "/" + UglyCurrentPage;
    public const string UglyAnyPageUrl = DefRoot + "/" + UglyAnyPage;

    public const string NiceAnyRelative = "/page{0}";
    public static string NiceAnyPageUrl = DefRoot + NiceAnyRelative;

    internal static string CurrentPageUrl = NiceCurrentUrl;
    internal static string AnyPageUrl = NiceAnyPageUrl;

    protected override string ToApi(string api, string parameters = null) => $"{api}{Parameters(parameters)}";

    protected override string ToPage(int? pageId, string parameters = null, string language = null) =>
        // Page or Api?
        pageId != null
            ? string.Format(AnyPageUrl, pageId) + Parameters(parameters)
            : $"{CurrentPageUrl}{Parameters(parameters)}";

    private static string Parameters(string parameters) => string.IsNullOrEmpty(parameters) ? parameters : $"?{parameters}";


    public static void SwitchModeToUgly(bool uglyOn)
    {
        CurrentPageUrl = uglyOn ? UglyCurrentUrl : NiceCurrentUrl;
        AnyPageUrl = uglyOn ? UglyAnyPageUrl : NiceAnyPageUrl;
    }

}