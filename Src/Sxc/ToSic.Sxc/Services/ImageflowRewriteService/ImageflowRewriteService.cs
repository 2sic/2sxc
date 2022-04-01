using System;
using System.Collections.Specialized;
using System.Linq;

namespace ToSic.Sxc.Services
{
    public class ImageflowRewriteService : IImageflowRewriteService
    {
        public static readonly string JpgQualityDefault = "70";
        public static readonly string PngQualityDefault = "60";
        public static readonly string WebpQualityDefault = "50";
        public static readonly string Quality = "quality";
        public static readonly string JpgQuality = "jpg.quality";
        public static readonly string PngQuality = "png.quality";
        public static readonly string WebpQuality = "webp.quality";
        public static readonly string Format = "format";

        public NameValueCollection QueryStringRewrite(NameValueCollection queryString)
        {
            if (queryString == null) queryString = new NameValueCollection();

            // TODO: eventual add "format" parameter if is missing

            queryString = AddQualityIfMissing(queryString);

            queryString = RewriteQuality(queryString);
            
            return queryString;
        }

        public static NameValueCollection AddKeyWhenMissing(NameValueCollection queryString, string key, string value)
        {
            if (string.IsNullOrEmpty(queryString[key]))
                queryString.Add(key, value);

            return queryString;
        }

        public static NameValueCollection AddQualityIfMissing(NameValueCollection queryString)
        {
            if (string.IsNullOrEmpty(queryString[Quality]))
            {
                switch (queryString[Format]?.ToLower())
                {
                    case "jpg":
                        AddKeyWhenMissing(queryString, JpgQuality, JpgQualityDefault);
                        break;
                    case "png":
                        AddKeyWhenMissing(queryString, PngQuality, PngQualityDefault);
                        break;
                    case "webp":
                        AddKeyWhenMissing(queryString, WebpQuality, WebpQualityDefault);
                        break;
                    default:
                        AddKeyWhenMissing(queryString, Quality, JpgQualityDefault);
                        break;
                }
            }
            return queryString;
        }

        public static NameValueCollection RewriteQuality(NameValueCollection queryString)
        {
            var qsNew = new NameValueCollection();

            foreach (var key in queryString.AllKeys)
            {
                var value = queryString[key];
                var newKey = key;

                // rewrite query string
                if (key.Equals(Quality, StringComparison.OrdinalIgnoreCase))
                {
                    // use the quality parameter of format,
                    // rather than reusing the jpeg quality command.
                    switch (queryString[Format]?.ToLower())
                    {
                        case "jpg":
                            newKey = JpgQuality;
                            break;
                        case "png":
                            newKey = PngQuality;
                            break;
                        case "webp":
                            newKey = WebpQuality;
                            break;
                        default:
                            newKey = key;
                            break;
                    }
                    if (!queryString.AllKeys.Contains(newKey))
                        qsNew[newKey] = value; // rewrite
                    else
                        qsNew[Quality] = value; // do not rewrite, because we already have quality parameters specified for format
                }
                else
                    qsNew[newKey] = value; // just copy
            }

            return qsNew;
        }
    }
}
