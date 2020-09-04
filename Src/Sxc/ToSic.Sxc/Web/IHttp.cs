
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

        /// <summary>
        /// Get a full path like C:\...
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string MapPath(string virtualPath);

        /// <summary>
        /// Get an absolute path like /Portals/xyz
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string ToAbsolute(string virtualPath);

        /// <summary>
        /// Get an Absolute path like /portal/xyz and combine with another relative path
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        string Combine(string basePath, string relativePath);
    }
}
