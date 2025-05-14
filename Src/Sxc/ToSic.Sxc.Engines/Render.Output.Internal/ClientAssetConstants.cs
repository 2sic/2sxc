using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Web.Internal.ClientAssets;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ClientAssetConstants
{
    #region Constants for placement in resulting HTML

    public const string AddToBody = "body";
    public const string AddToHead = "head";
    public const string AddToBottom = "bottom";

    #endregion

    public const int CssDefaultPriority = 99;
    public const int JsDefaultPriority = 100;

    public const string AttributeDefer = "defer";
    public const string AttributeAsync = "async";

    public const string AssetOptimizationsAttributeName = "data-enableoptimizations";

    /// <summary>
    /// List of special attributes like "src", "id", "data-enableoptimizations"
    /// that we need to skip from adding in general HtmlAttributes dictionary
    /// because this special attributes are handled in custom way.
    /// </summary>
    internal static readonly List<string> SpecialHtmlAttributes =
    [
        "src",
        "id",
        AssetOptimizationsAttributeName,
        CspConstants.CspWhitelistAttribute
    ];
}