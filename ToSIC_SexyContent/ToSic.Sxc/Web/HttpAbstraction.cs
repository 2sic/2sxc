#if NET451
using System.Web;
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
#endif
using System;
using System.Collections.Specialized;
using System.IO;

namespace ToSic.Sxc.Web
{
    public class HttpAbstraction: IHttp
    {
#if NET451
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        public HttpAbstraction() => Current = HttpContext.Current;

#else
        public HttpAbstraction(IHttpContextAccessor contextAccessor, IHostingEnvironment hostingEnvironment, IUrlHelper urlHelper)
        {
            Current = contextAccessor.HttpContext;
            _hostingEnvironment = hostingEnvironment;
            _urlHelper = urlHelper;
        }

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUrlHelper _urlHelper;
#endif

        public HttpContext Current { get; }

        #region Request and properties thereof
        public HttpRequest Request => Current?.Request;

        public NameValueCollection QueryString
        {
            get
            {
#if NET451
                    return Request.QueryString;
#else
                    var paramList = new NameValueCollection();
                    Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
                    return paramList;
#endif
            }
        }
        #endregion Request

        #region MapPath and other Paths

        public string MapPath(string virtualPath)
        {
#if NETSTANDARD
            return Path.Combine(_hostingEnvironment.ContentRootPath, virtualPath);
#else
            return HostingEnvironment.MapPath(virtualPath);
#endif
        }

        public string ToAbsolute(string virtualPath)
        {
#if NETSTANDARD
            return _urlHelper.Content(virtualPath);
#else
            return VirtualPathUtility.ToAbsolute(virtualPath);
#endif
        }
        public string Combine(string basePath, string relativePath)
        {
#if NETSTANDARD
            return _urlHelper.Content(Path.Combine(basePath, relativePath));
#else
            return VirtualPathUtility.Combine(basePath, relativePath);
#endif
        }


        #endregion
    }
}
