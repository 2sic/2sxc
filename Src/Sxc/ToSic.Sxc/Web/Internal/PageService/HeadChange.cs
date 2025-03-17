using ToSic.Razor.Blade;

namespace ToSic.Sxc.Web.Internal.PageService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public struct HeadChange
{
    public PageChangeModes ChangeMode { get; init; }

    public IHtmlTag Tag { get; init; }

    /// <summary>
    /// This is part of the original property, which would be replaced.
    /// </summary>
    public string ReplacementIdentifier { get; init; }
}