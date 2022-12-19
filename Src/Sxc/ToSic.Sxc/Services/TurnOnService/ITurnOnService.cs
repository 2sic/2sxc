using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Services
{
    [PrivateApi("WIP")]
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
}
