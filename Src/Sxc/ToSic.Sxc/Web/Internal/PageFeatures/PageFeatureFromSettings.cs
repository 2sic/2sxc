namespace ToSic.Sxc.Web.Internal.PageFeatures;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public record PageFeatureFromSettings : PageFeature
{
    /// <summary>
    /// Toggles information if it was already processed.
    /// Must be settable, as the list may be reviewed again and should not pick up
    /// things which were already processed.
    /// </summary>
    public bool AlreadyProcessed { get; set; }

    public bool AutoOptimize { get; init; }
}