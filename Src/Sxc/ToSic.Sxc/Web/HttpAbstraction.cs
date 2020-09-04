#if NET451
using System.Web;
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
#endif
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;


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
                if (_queryStringValues != null) return _queryStringValues;
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if(Request == null) return _queryStringValues = new NameValueCollection();
#if NET451
                return _queryStringValues = Request.QueryString;
#else
                var paramList = new NameValueCollection();
                Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
                return _queryStringValues = paramList;
#endif
            }
        }

        private NameValueCollection _queryStringValues;

        public List<KeyValuePair<string, string>> QueryStringKeyValuePairs()
        {
            if (_queryStringKeyValuePairs != null) return _queryStringKeyValuePairs;
            var qs = QueryString;
            _queryStringKeyValuePairs = qs?.AllKeys
                                            .Select(key => new KeyValuePair<string, string>(key, qs[key]))
                                            .ToList()
                                        ?? new List<KeyValuePair<string, string>>();
            return _queryStringKeyValuePairs;
            //return (from string key in qs select new KeyValuePair<string, string>(key, qs[key]))
            //    .ToList();
        }

        private List<KeyValuePair<string, string>> _queryStringKeyValuePairs;
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
