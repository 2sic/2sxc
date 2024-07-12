using ToSic.Razor.Blade;
using Attribute = ToSic.Razor.Markup.Attribute;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// turnOn Service helps initialize / boot JavaScripts when all requirements (usually dependencies) are ready.
/// </summary>
[PrivateApi("Don't publish yet - the functionality is surfaced on the PageService!")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ITurnOnService: IHasLog
{

    Attribute Attribute(object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = null,
        object data = null);

    IHtmlTag Run(object runOrSpecs,
        NoParamOrder noParamOrder = default,
        object require = null,
        object data = null,
        IEnumerable<object> args = default,
        string addContext = default
    );
}