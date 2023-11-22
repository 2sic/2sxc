using System;
using System.Web;
using ToSic.Lib.Helpers;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [Serializable]
    public class HttpContextProxy : MarshalByRefObject
    {
        private HttpContext _currentHttpContext = HttpContext.Current;
        //public dynamic GetCurrentHttpContext() => _currentHttpContext;
        public void SyncContext()
        {
            if (HttpContext.Current == null)
                HttpContext.Current = _currentHttpContext;
        }

        //public HttpContextBase ContextWrapper => _httpContext.Get(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
        //private readonly GetOnce<HttpContextBase> _httpContext = new GetOnce<HttpContextBase>();
    }

}
