namespace ToSic.Sxc.Web.Internal.ClientAssets;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ClientAssetsExtractSettings
{
    public ClientAssetsExtractSettings(
        bool extractAll = false,
        int cssPriority = default,
        string cssLocation = default,
        int jsPriority = default,
        string jsLocation = default,
        ClientAssetExtractSettings js = default,
        ClientAssetExtractSettings css = default)
    {
        var jsLoc = jsLocation ?? ClientAssetConstants.AddToBody;
        var cssLoc = cssLocation ?? ClientAssetConstants.AddToHead;
        var jsPrio = jsPriority == default ? ClientAssetConstants.JsDefaultPriority : jsPriority;
        var cssPrio = cssPriority == default ? ClientAssetConstants.CssDefaultPriority : cssPriority;

        Css = css ?? new ClientAssetExtractSettings(extractAll, cssLoc, cssPrio, false, false);
        Js = js ?? new ClientAssetExtractSettings(extractAll, jsLoc, jsPrio, false, false);
    }

    public ClientAssetExtractSettings Css { get; }

    public ClientAssetExtractSettings Js { get; }
}