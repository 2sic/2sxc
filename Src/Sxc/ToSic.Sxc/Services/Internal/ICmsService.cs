using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("not published, use Item.Html() instead")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICmsService
{
    IHtmlTag Html(object thing,
        NoParamOrder noParamOrder = default,
        object container = default,
        string classes = default,
        bool debug = default,
        object imageSettings = default,
        bool? toolbar = default,
        Func<ITweakInput<string>, ITweakInput<string>> tweak = default);
}