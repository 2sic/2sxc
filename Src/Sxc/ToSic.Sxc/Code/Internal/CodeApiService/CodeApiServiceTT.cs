using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class CodeApiService<TModel, TServiceKit>(CodeApiService.MyServices services, string logPrefix)
    : CodeApiService(services, logPrefix), ICodeApiService<TModel, TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
    //public TModel Model => default;

    public TServiceKit Kit => _kit.Get(GetService<TServiceKit>);
    private readonly GetOnce<TServiceKit> _kit = new();
}