#if NETFRAMEWORK
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlEncoder = System.Text.Encodings.Web.HtmlEncoder;
#endif
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Web.Sys.Html;

/// <summary>
/// Cross-platform (.net core and framework) HTML string implementation
///
/// IMPORTANT: this is a 1:1 copy of the RawHtmlString in RazorBlade, but we needed a `record` implementation
/// </summary>
[PrivateApi]
public record HybridHtmlString : IRawHtmlString
{
    /// <summary>
    /// Constructor to provide initial value.
    /// </summary>
    /// <param name="value"></param>
    public HybridHtmlString(string value) => _value = value;

    /// <summary>
    /// Constructor with empty initial value.
    /// Mainly used when overriding this class, and probably never using _value.
    /// </summary>
    protected HybridHtmlString() => _value = "";
    private readonly string _value;

    /// <summary>
    /// Records automatically overwrite ToString(),
    /// so we must use another method for providing the desired HTML string.
    ///
    /// This is also the method used for any other integration, so overwrite this if you need to change the behavior.
    /// </summary>
    /// <returns></returns>
    protected virtual string ToHtmlString() => _value;

#if NETFRAMEWORK
    /// <summary>
    /// This is the serialization for the old-style asp.net razor
    /// </summary>
    /// <returns></returns>
    [PrivateApi]
    string IHtmlString.ToHtmlString() => ToHtmlString();
#else

        /// <inheritdoc />
        [PrivateApi]
        public void WriteTo(System.IO.TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null) throw new System.ArgumentNullException(nameof(writer));
            writer.Write(ToHtmlString());
        }
#endif

    //public static bool IsStringOrHtmlString(object original, out string asString)
    //{
    //    asString = null;
    //    if (original is null) return false;
    //    if (original is string strOriginal)
    //    {
    //        asString = strOriginal;
    //        return true;
    //    }
    //    if (original is IHtmlString)
    //    {
    //        asString = original.ToString();
    //        return true;
    //    }
    //    return false;
    //}
}