using System.Collections.Specialized;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Oqt.Server.Plumbing;

/// <summary>
/// This is responsible for providing the current HTTP context to the application.
/// In addition, it also provides the Query-String params (which are hidden in the normal request)
/// </summary>
/// <remarks>
/// Note that it's probably not yet complete. As of now, it only provides ?x=y&a=b stuff,
/// but if Blazor has any cute-url system like x/y/a/b this would probably not work yet
/// </remarks>
internal class HttpBlazor : HttpAbstractionBase, IHttp
{
    private readonly NavigationManager _navigationManager;

    public HttpBlazor(IHttpContextAccessor contextAccessor, NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        Current = contextAccessor?.HttpContext;
    }

    public override NameValueCollection QueryStringParams
    {
        get
        {
            // caching is disabled because in Blazor Interactive the query string parameters are changed after the page is created
            //if (_queryStringValues != null) return _queryStringValues;

            // this must behave differently in an API call, as the navigation manager will not be initialized
            // but all the params will really be in the query
            if (Current?.Request?.Path.Value?.Contains("_blazor") == false)
            {
                var paramList = new NameValueCollection();
                Current.Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
                return /*_queryStringValues = */FilterOutOqtaneParams(paramList);
            }
            else
            {

                // Sample taken from https://chrissainty.com/working-with-query-strings-in-blazor/
                var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
                var queryBits = QueryHelpers.ParseQuery(uri.Query);
                    
                if (!queryBits.Any()) 
                    return /*_queryStringValues = */[];

                var paramList = new NameValueCollection();
                queryBits.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
                return /*_queryStringValues = */FilterOutOqtaneParams(paramList);
            }
        }
    }

    //private NameValueCollection _queryStringValues;

    private NameValueCollection FilterOutOqtaneParams(NameValueCollection original)
    {
        // Oqtane seems to add this automatically, at least in v2.2
        original.Remove("authmoduleid");
        return original;
    }

    //public override IDictionary<object, object> Items => Current.Items;
}