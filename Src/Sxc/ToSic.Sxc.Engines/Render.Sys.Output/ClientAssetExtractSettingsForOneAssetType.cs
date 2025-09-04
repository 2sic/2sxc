namespace ToSic.Sxc.Render.Sys.Output;

/// <summary>
/// 
/// </summary>
/// <param name="ExtractAll">Extract all assets, even if they don't have an auto-optimize attribute.</param>
/// <param name="Location"></param>
/// <param name="Priority">Default Priority - will be used for sorting when added to page</param>
/// <param name="AutoDefer">Automatically add a `defer` attribute to scripts</param>
/// <param name="AutoAsync">Automatically add as `async` attribute to scripts</param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public record ClientAssetExtractSettingsForOneAssetType(bool ExtractAll, string Location, int Priority, bool AutoDefer, bool AutoAsync);