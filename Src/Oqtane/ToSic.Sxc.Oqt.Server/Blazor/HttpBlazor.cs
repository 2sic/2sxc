using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Blazor
{
    /// <summary>
    /// This is responsible for providing the current HTTP context to the application.
    /// In addition, it also provides the Query-String params (which are hidden in the normal request)
    /// </summary>
    /// <remarks>
    /// Note that it's probably not yet complete. As of now, it only provides ?x=y&a=b stuff,
    /// but if Blazor has any cute-url system like x/y/a/b this would probably not work yet
    /// </remarks>

    public class HttpBlazor : HttpAbstractionBase, IHttp
    {
        private readonly NavigationManager _navigationManager;

        public HttpBlazor(IHttpContextAccessor contextAccessor, NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            Current = contextAccessor.HttpContext;
        }

        public override NameValueCollection QueryStringParams
        {
            get
            {
                if (_queryStringValues != null) return _queryStringValues;

                // Sample taken from https://chrissainty.com/working-with-query-strings-in-blazor/
                var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
                var queryBits = QueryHelpers.ParseQuery(uri.Query);
                
                if (!queryBits.Any()) 
                    return _queryStringValues = new NameValueCollection();

                var paramList = new NameValueCollection();
                queryBits.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
                return _queryStringValues = paramList;
            }
        }

        private NameValueCollection _queryStringValues;


    }
}
