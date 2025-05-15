using ToSic.Razor.Markup;

namespace ToSic.Sxc.Data;

public partial interface ITyped
{
    /// <summary>
    /// Return a value as a raw HTML string for using inside an attribute.
    /// Usage like `title='@item.Attribute("Title")'`
    /// It will do a few things such as:
    /// 
    /// 1. Ensure dates are in the ISO format
    /// 1. Ensure numbers are in a neutral format such as `14.27` and never `14,27`
    /// 1. Html encode any characters which would cause trouble such as quotes
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="fallback">Value to use if the property specified by `name` doesn't exist</param>
    /// <param name="required">throw error if `name` doesn't exist, see [](xref:NetCode.Conventions.PropertiesRequired)</param>
    /// <returns></returns>
    IRawHtmlString Attribute(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default);
}