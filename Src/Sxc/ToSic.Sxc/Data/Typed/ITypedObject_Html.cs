using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedObject
    {
        IRawHtmlString Attribute(string name, string noParamOrder = ToSic.Eav.Parameters.Protector, string attribute = default);
    }
}
