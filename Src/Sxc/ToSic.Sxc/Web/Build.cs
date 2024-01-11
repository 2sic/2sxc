using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web;

[PrivateApi("internal use only, may be removed/changed some day")]
internal class Build
{
    /// <summary>
    /// Generate an HTML attribute
    /// - but only if in edit mode
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// has trailing space to ensure attributes never "stick" together
    /// we also don't want to use {HttpUtility.HtmlAttributeEncode(value)...
    /// ...because it makes the html hard to work with when debugging
    /// so we just manually replace all apos to make sure it doesn't create invalid html
    /// </remarks>
    public static IRawHtmlString Attribute(string name, string value)
        => new RawHtmlString($" {Tag.Attr(name, value)} ");
}