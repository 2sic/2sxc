using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Services;

/// <summary>
/// turnOn Service helps initialize / boot JavaScripts when all requirements (usually dependencies) are ready.
/// </summary>
[PrivateApi("Don't publish yet - the functionality is surfaced on the PageService!")]
public interface ITurnOnService: IHasLog
{

    Attribute Attribute(object runOrSpecs,
        string noParamOrder = Eav.Parameters.Protector,
        object require = null,
        object data = null);

    IHtmlTag Run(object runOrSpecs,
        string noParamOrder = Eav.Parameters.Protector,
        object require = null,
        object data = null);
}