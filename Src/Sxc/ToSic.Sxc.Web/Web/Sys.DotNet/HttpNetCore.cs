#if !NETFRAMEWORK
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Web.Sys.Http;

namespace ToSic.Sxc.Web.Internal.DotNet;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpNetCore(IHttpContextAccessor contextAccessor) : HttpAbstractionBase, IHttp
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