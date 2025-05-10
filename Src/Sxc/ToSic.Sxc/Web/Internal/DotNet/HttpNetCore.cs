#if !NETFRAMEWORK
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;

namespace ToSic.Sxc.Web.Internal.DotNet;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpNetCore : HttpAbstractionBase, IHttp
{
    public HttpNetCore(IHttpContextAccessor contextAccessor) => Current = contextAccessor.HttpContext;

    public override NameValueCollection QueryStringParams
    {
        get
        {
            if (field != null) return field;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (Request == null) 
                return field = [];

            var paramList = new NameValueCollection();
            Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
            return field = paramList;
        }
    }
}

#endif