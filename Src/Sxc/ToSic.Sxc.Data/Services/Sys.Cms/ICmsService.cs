using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Services.Sys.Cms;

[PrivateApi("not published, use Item.Html() instead")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICmsService
{
    IHtmlTag Html(object thing,
        NoParamOrder npo = default,
        object? container = default,
        string? classes = default,
        bool debug = default,
        object? imageSettings = default,
        bool? toolbar = default,
        Func<ITweakInput<string>, ITweakInput<string>>? tweak = default);
}