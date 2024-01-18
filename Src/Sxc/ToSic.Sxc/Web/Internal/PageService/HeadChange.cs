using ToSic.Razor.Blade;

namespace ToSic.Sxc.Web.Internal.PageService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public struct HeadChange
{
    public PageChangeModes ChangeMode { get; set; }

    public IHtmlTag Tag { get; set; }

    /// <summary>
    /// This is part of the original property, which would be replaced.
    /// </summary>
    public string ReplacementIdentifier { get; set; }
}