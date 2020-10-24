
using System.Collections.Generic;
using System.Collections.Specialized;
#if NET451
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Goal is that anything on this will be able to provide HttpContext operations as needed
    /// To abstract .net451 and .net core
    /// </summary>
    public interface IHttp
    {
        HttpContext Current { get; }

        HttpRequest Request { get; }

        NameValueCollection QueryString { get; }

        List<KeyValuePair<string, string>> QueryStringKeyValuePairs();
    }
}
