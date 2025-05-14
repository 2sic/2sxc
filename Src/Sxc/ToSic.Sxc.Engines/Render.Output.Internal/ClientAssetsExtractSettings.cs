namespace ToSic.Sxc.Web.Internal.ClientAssets;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ClientAssetsExtractSettings
{
    public ClientAssetsExtractSettings(
        bool extractAll = false,
        int cssPriority = default,
        string cssLocation = default,
        int jsPriority = default,
        string jsLocation = default,
        ClientAssetExtractSettingsForOneAssetType js = default,
        ClientAssetExtractSettingsForOneAssetType css = default)
    {
        var jsLoc = jsLocation ?? ClientAssetConstants.AddToBody;
        var cssLoc = cssLocation ?? ClientAssetConstants.AddToHead;
        var jsPrio = jsPriority == default ? ClientAssetConstants.JsDefaultPriority : jsPriority;
        var cssPrio = cssPriority == default ? ClientAssetConstants.CssDefaultPriority : cssPriority;

        Css = css ?? new ClientAssetExtractSettingsForOneAssetType(extractAll, cssLoc, cssPrio, false, false);
        Js = js ?? new ClientAssetExtractSettingsForOneAssetType(extractAll, jsLoc, jsPrio, false, false);
    }

    public ClientAssetExtractSettingsForOneAssetType Css { get; }

    public ClientAssetExtractSettingsForOneAssetType Js { get; }
}