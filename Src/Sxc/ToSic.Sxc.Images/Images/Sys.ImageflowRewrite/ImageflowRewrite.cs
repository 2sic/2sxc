using System.Collections.Specialized;

namespace ToSic.Sxc.Images;

// Is executed in dnn imageflow module and oqtane imageflow module to
// rewrite query string params before imageflow middleware take a care of them.
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ImageflowRewrite
{
    public static readonly string Quality = "quality";
    //public static readonly string JpgQuality = "jpg.quality";
    public static readonly string PngQuality = "png.quality";
    public static readonly string WebpQuality = "webp.quality";

    [return: NotNullIfNotNull(nameof(queryString))]
    public static NameValueCollection? QueryStringRewrite(NameValueCollection? queryString)
    {
        // rewrite query string
        if (queryString != null && !string.IsNullOrEmpty(queryString[Quality]))
        {
            var value = queryString[Quality];
            // use the quality parameter of format,
            // rather than reusing the jpeg quality command.
            AddKeyWhenMissing(queryString, PngQuality, value);
            AddKeyWhenMissing(queryString, WebpQuality, value);
        }
        return queryString;
    }

    public static NameValueCollection AddKeyWhenMissing(NameValueCollection queryString, string key, string? value)
    {
        if (string.IsNullOrEmpty(queryString[key]))
            queryString.Add(key, value);
        return queryString;
    }
}