using System;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Services;

[PrivateApi("WIP")]
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