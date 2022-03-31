using System.Collections.Specialized;

namespace ToSic.Sxc.Services
{
    public interface IImageflowRewriteService
    {
        NameValueCollection QueryStringRewrite(NameValueCollection queryString);
    }
}