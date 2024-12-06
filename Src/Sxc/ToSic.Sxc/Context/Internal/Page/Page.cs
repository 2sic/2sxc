using ToSic.Lib.DI;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// Constructor for DI
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class Page(LazySvc<IHttp> httpLazy) : IPage
{
    public IPage Init(int id)
    {
        Id = id;
        return this;
    }

    public int Id { get; private set; } = Eav.Constants.NullId;

    public virtual IParameters Parameters => field ??= new Parameters { Nvc = OriginalParameters.GetOverrideParams(httpLazy.Value?.QueryStringParams) };


    public string Url { get; set; } = Eav.Constants.UrlNotInitialized;
}