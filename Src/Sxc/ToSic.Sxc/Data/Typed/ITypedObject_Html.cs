using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data
{
    public partial interface ITyped
    {
        /// <summary>
        /// Return a value as a raw HTML string for using inside an attribute.
        /// Usage eg. `title='@item.Attribute("Title")'`
        /// It will do a few things such as:
        ///
        /// 1. Ensure dates are in the ISO format
        /// 1. Ensure numbers are in a neutral format such as `14.27` and never `14,27`
        /// 1. Html encode any characters which would cause trouble such as quotes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        IRawHtmlString Attribute(string name, string noParamOrder = ToSic.Eav.Parameters.Protector, string fallback = default);
    }
}
