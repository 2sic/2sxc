namespace ToSic.Sxc.Web.Internal.PageService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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