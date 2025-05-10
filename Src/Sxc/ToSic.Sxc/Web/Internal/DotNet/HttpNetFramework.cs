#if NETFRAMEWORK
using System.Collections.Specialized;
using System.Web;

namespace ToSic.Sxc.Web.Internal.DotNet;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpNetFramework: HttpAbstractionBase, IHttp
{
    /// <summary>
    /// Empty constructor for DI
    /// </summary>
    public HttpNetFramework() => Current = HttpContext.Current;

    #region Request and properties thereof

    public override NameValueCollection QueryStringParams => field ??= Request?.QueryString ?? [];

    #endregion Request

}
#endif