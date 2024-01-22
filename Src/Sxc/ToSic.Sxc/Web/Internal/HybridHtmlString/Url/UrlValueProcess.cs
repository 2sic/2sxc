using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Web.Internal.Url;

/// <summary>
/// Base class for processing URL Values before keeping / converting to a string-url
/// </summary>
internal abstract class UrlValueProcess
{
    public abstract NameObjectSet Process(NameObjectSet set);

    // Base64 marker for rule encoding
    public static string Base64Prefix = "base64:";
    public static string Json64Prefix = "json64:";

    public static char[] UnsafeChars =
    [
        '\n', '\r',
        '<', '>',
        '"', '\'',
        '=', '&', '?', '#'
    ];

    /// <summary>
    /// Converts any string value which contains unsafe characters to base64
    /// Works for SVG icons and similar
    /// Requires the receiving system (in this case the inpage JS) to handle strings starting with "base64:" differently. 
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    protected NameObjectSet MakeSafe(NameObjectSet set)
    {
        var obj = set.Value;
        return obj is string str && str.HasValue() && UnsafeChars.Any(c => str.Contains(c))
            ? new(set, value: $"{Base64Prefix}{Base64.Encode(str)}")
            : set;
    }

}