using ToSic.Razor.Blade;

namespace ToSic.Sxc.Render.Output.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record ModuleHtml
{
    public List<IHtmlTag> HtmlTags { get; init; } = [];
    public HashSet<string> DistinctTags { get; init; } = [];
}
