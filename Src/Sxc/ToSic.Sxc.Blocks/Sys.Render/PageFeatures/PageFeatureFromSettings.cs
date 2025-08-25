namespace ToSic.Sxc.Sys.Render.PageFeatures;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record PageFeatureFromSettings : PageFeature
{
    public bool AutoOptimize { get; init; }
}