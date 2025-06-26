using ToSic.Razor.Blade;

namespace ToSic.Sxc.Sys.Render.PageContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public struct HeadChange
{
    public PageChangeModes ChangeMode { get; init; }

    public IHtmlTag Tag { get; init; }

    /// <summary>
    /// This is part of the original property, which would be replaced.
    /// </summary>
    public string? ReplacementIdentifier { get; init; }
}