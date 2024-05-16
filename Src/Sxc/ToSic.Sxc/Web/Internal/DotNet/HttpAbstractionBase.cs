#if NETFRAMEWORK
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif
using System.Collections.Specialized;


namespace ToSic.Sxc.Web.Internal.DotNet;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class HttpAbstractionBase: IHttp
{
    /// <inheritdoc />
    public HttpContext Current { get; protected set; }

    #region Request and properties thereof

    /// <inheritdoc />
    public HttpRequest Request => Current?.Request;

    /// <inheritdoc />
    public abstract NameValueCollection QueryStringParams { get; }

    /// <inheritdoc />
    public List<KeyValuePair<string, string>> QueryStringKeyValuePairs()
    {
        if (_queryStringKeyValuePairs != null) return _queryStringKeyValuePairs;
        var qs = QueryStringParams;
        _queryStringKeyValuePairs = qs?.AllKeys
                                        .Select(key => new KeyValuePair<string, string>(key, qs[key]))
                                        .ToList()
                                    ?? [];
        return _queryStringKeyValuePairs;
    }
    private List<KeyValuePair<string, string>> _queryStringKeyValuePairs;


    public string GetCookie(string cookieName)
    {
        if (Request == null) return null;

#if NETFRAMEWORK
        return Request.Cookies[cookieName]?.Value;
#else
        return Request.Cookies[cookieName];
#endif
    }

    #endregion Request

    //public abstract IDictionary<object, object> Items { get; }

}