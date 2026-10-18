using ToSic.Eav.Sys;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sxc.Web.Sys.Parameters;

namespace ToSic.Sxc.Context.Sys.Page;

/// <summary>
/// Constructor for DI
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Page(LazySvc<IHttp> httpLazy) : IPage
{
    public int Id { get; protected set; } = EavConstants.NullId;

    [field: AllowNull, MaybeNull]
    public virtual IParameters Parameters => field ??= new Parameters { Nvc = OriginalParameters.GetOverrideParams(httpLazy.Value?.QueryStringParams!) };


    public string Url { get; set; } = EavConstants.UrlNotInitialized;
}