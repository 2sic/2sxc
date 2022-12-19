using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.CmsService
{
    [PrivateApi("WIP")]
    public interface ICmsService
    {
        IHtmlTag Show(object thing, string noParamOrder = Eav.Parameters.Protector, object container = null);
    }
}
