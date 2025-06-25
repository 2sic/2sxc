#if NETFRAMEWORK
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using ToSic.Sxc.Web.Sys.Http;

namespace ToSic.Sxc.Web.Internal.DotNet;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpNetFramework: HttpAbstractionBase, IHttp
{
    public override HttpContext Current => HttpContext.Current!;

    #region Request and properties thereof

    [field: AllowNull, MaybeNull]
    public override NameValueCollection QueryStringParams
        => field ??= Request?.QueryString ?? [];

    #endregion Request

}
#endif