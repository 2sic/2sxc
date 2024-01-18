namespace ToSic.Sxc.Web.Internal.PageFeatures;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageFeatureFromSettings(string key, bool autoOptimize = default, string[] needs = null, string html = null)
    : PageFeature(key, "", "", needs, html)
{
    public bool AlreadyProcessed { get; set; }

    public bool AutoOptimize { get; } = autoOptimize;
}