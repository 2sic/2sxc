namespace ToSic.Sxc.Web.Internal.PageService;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record PagePropertyChange
{
    public PageChangeModes ChangeMode { get; init; }
        
    internal PageProperties Property { get; init; }

    public string Value { get; init; }

    /// <summary>
    /// This is part of the original property, which would be replaced.
    /// </summary>
    public string ReplacementIdentifier { get; init; }
}