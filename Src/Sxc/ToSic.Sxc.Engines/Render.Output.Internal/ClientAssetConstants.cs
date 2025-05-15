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


}