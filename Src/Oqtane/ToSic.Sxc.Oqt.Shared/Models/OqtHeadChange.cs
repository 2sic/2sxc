namespace ToSic.Sxc.Oqt.Shared.Models;

// Equivalent to ToSic.Sxc.Web.PageService.HeadChange
[ShowApiWhenReleased(ShowApiMode.Never)]
public class OqtHeadChange
{
    public required OqtPagePropertyOperation PropertyOperation { get; init; }

    public required string Tag { get; init; }

    /// <summary>
    /// This is part of the original property, which would be replaced.
    /// </summary>
    public required string ReplacementIdentifier { get; init; }
}