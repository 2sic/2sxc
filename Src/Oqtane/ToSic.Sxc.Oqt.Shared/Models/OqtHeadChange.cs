namespace ToSic.Sxc.Oqt.Shared.Models;

// Equivalent to ToSic.Sxc.Web.PageService.HeadChange
[ShowApiWhenReleased(ShowApiMode.Never)]
public class OqtHeadChange
{
    public OqtPagePropertyOperation PropertyOperation { get; set; }
    public string Tag { get; set; }

    /// <summary>
    /// This is part of the original property, which would be replaced.
    /// </summary>
    public string ReplacementIdentifier { get; set; }
}