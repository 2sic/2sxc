
using System.Collections.Generic;
using System.Collections.Specialized;
using ToSic.Lib.Documentation;
#if NETFRAMEWORK
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
    [PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice, hidden 2021-12")]
    public interface IHttp
    {
        /// <summary>
        /// The standardized HttpContext. It's type changes depending on the framework it's running in
        /// </summary>
        HttpContext Current { get; }

        /// <summary>
        /// The standardized" HttpRequest object. It's type changes depending on the framework it's running in.
        /// </summary>
        HttpRequest Request { get; }

        /// <summary>
        /// The standardized QueryString parameters so it works on all platforms
        /// </summary>
        NameValueCollection QueryStringParams { get; }

        /// <summary>
        /// QueryString params as KeyValue Pairs.
        /// We don't use a dictionary, because sometimes the same key can occur twice.
        /// </summary>
        /// <returns></returns>
        List<KeyValuePair<string, string>> QueryStringKeyValuePairs();
    }
}
