namespace ToSic.Sxc.Web.ClientAssets;

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

public class ClientAssetExtractSettings
{
    public ClientAssetExtractSettings(bool extractAll, string location, int priority, bool autoDefer, bool autoAsync)
    {
        ExtractAll = extractAll;
        Location = location;
        Priority = priority;
        AutoDefer = autoDefer;
        AutoAsync = autoAsync;
    }
    /// <summary>
    /// Extract all assets, even if they don't have an auto-optimize attribute.
    /// </summary>
    public bool ExtractAll { get; }
    public string Location { get; }

    /// <summary>
    /// Default Priority - will be used for sorting when added to page
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Automatically add a `defer` attribute to scripts
    /// </summary>
    public bool AutoDefer { get; }


    /// <summary>
    /// Automatically add as `async` attribute to scripts
    /// </summary>
    public bool AutoAsync { get; }
}