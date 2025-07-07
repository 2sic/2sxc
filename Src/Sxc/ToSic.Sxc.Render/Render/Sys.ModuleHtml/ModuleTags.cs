using ToSic.Razor.Blade;

namespace ToSic.Sxc.Render.Sys.ModuleHtml;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record ModuleTags
{
    public List<IHtmlTag> MoreTags { get; init; } = [];
    public HashSet<string> ExistingKeys { get; init; } = [];
}
