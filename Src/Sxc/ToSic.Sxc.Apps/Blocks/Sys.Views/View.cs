using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Blocks.Sys.Views;

/// <summary>
/// Instance of a view; extending the ViewConfiguration with values which are only relevant for just now.
/// </summary>
[PrivateApi("Internal implementation - don't publish")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record View : ViewConfiguration, IView
{
    public string? Edition { get; set; }

    public string? EditionPath { get; set; }

    public bool IsReplaced { get; init; } = false;

    public IMetadata Metadata => Entity.Metadata;
}
