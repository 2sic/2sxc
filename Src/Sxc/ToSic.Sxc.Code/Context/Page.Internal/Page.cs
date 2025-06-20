using ToSic.Eav.Sys;
using ToSic.Lib.DI;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// Constructor for DI
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Page(LazySvc<IHttp> httpLazy) : IPage
{
    public IPage Init(int id)
    {
        Id = id;
        return this;
    }

    public int Id { get; private set; } = EavConstants.NullId;

    [field: AllowNull, MaybeNull]
    public virtual IParameters Parameters => field ??= new Parameters { Nvc = OriginalParameters.GetOverrideParams(httpLazy.Value?.QueryStringParams!) };


    public string Url { get; set; } = EavConstants.UrlNotInitialized;
}