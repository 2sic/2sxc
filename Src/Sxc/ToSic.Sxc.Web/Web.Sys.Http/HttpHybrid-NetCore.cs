#if !NETFRAMEWORK
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace ToSic.Sxc.Web.Sys.Http;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpHybrid(IHttpContextAccessor contextAccessor) : HttpAbstractionBase, IHttp
{
    [field: AllowNull, MaybeNull]
    public override HttpContext Current => field ??= contextAccessor.HttpContext!;

    [field: AllowNull, MaybeNull]
    public override NameValueCollection QueryStringParams
    {
        get
        {
            if (field != null)
                return field;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (Request == null!) 
                return field = [];

            var paramList = new NameValueCollection();
            Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
            return field = paramList;
        }
    }
}

#endif