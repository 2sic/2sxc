using System;
using System.Collections.Specialized;

namespace ToSic.Sxc.Services
{
    public class ImageflowRewriteService : IImageflowRewriteService
    {
        public NameValueCollection QueryStringRewrite(NameValueCollection queryString)
        {
            var qsNew = new NameValueCollection();

            foreach (var key in queryString.AllKeys)
            {
                var value = queryString[key];
                var newKey = key;
                
                // rewrite query string
                if (key.Equals("quality", StringComparison.OrdinalIgnoreCase))
                {
                    // use the quality parameter of format,
                    // rather than reusing the jpeg quality command.
                    switch (queryString["format"].ToLower())
                    {
                        case "jpg":
                            newKey = "jpeg.quality";
                            break;
                        case "png":
                            newKey = "png.quality";
                            break;
                        case "webp":
                            newKey = "webp.quality";
                            break;
                        default:
                            newKey = key;
                            break;
                    }
                }
                
                qsNew.Add(newKey, value);
            }
            return qsNew;
        }
    }
}
