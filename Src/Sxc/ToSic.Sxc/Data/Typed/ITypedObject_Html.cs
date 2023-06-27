using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedRead
    {
        IRawHtmlString Attribute(string name, string noParamOrder = ToSic.Eav.Parameters.Protector, string fallback = default, string attribute = default);
    }
}
