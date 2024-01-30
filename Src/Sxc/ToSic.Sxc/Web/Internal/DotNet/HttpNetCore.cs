#if !NETFRAMEWORK
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;

namespace ToSic.Sxc.Web.Internal.DotNet;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HttpNetCore : HttpAbstractionBase, IHttp
{
    public HttpNetCore(IHttpContextAccessor contextAccessor) => Current = contextAccessor.HttpContext;

    public override NameValueCollection QueryStringParams
    {
        get
        {
            if (_queryStringValues != null) return _queryStringValues;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (Request == null) 
                return _queryStringValues = [];

            var paramList = new NameValueCollection();
            Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
            return _queryStringValues = paramList;
        }
    }
    private NameValueCollection _queryStringValues;

}

#endif