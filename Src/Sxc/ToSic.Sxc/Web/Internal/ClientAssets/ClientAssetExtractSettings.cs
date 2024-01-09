namespace ToSic.Sxc.Web.Internal.ClientAssets;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ClientAssetExtractSettings(bool extractAll, string location, int priority, bool autoDefer, bool autoAsync)
{
    /// <summary>
    /// Extract all assets, even if they don't have an auto-optimize attribute.
    /// </summary>
    public bool ExtractAll { get; } = extractAll;

    public string Location { get; } = location;

    /// <summary>
    /// Default Priority - will be used for sorting when added to page
    /// </summary>
    public int Priority { get; } = priority;

    /// <summary>
    /// Automatically add a `defer` attribute to scripts
    /// </summary>
    public bool AutoDefer { get; } = autoDefer;

    /// <summary>
    /// Automatically add as `async` attribute to scripts
    /// </summary>
    public bool AutoAsync { get; } = autoAsync;
}