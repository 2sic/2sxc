using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services
{
    [PrivateApi("WIP")]
    public interface ICmsService
    {
        IHtmlTag Html(object thing,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            string classes = default,
            bool debug = default,
            object imageSettings = default,
            bool? toolbar = default
        );
    }
}
