using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class CodeApiService<TModel, TServiceKit>(CodeApiService.MyServices services, string logPrefix)
    : CodeApiService(services, logPrefix), ICodeApiService<TModel, TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
    // Model not in use ATM, may never be. 
    //public TModel Model => default;

    /// <summary>
    /// The primary kit for this service.
    /// Other kit versions can be accessed using `GetKit{TKit}`
    /// </summary>
    public TServiceKit Kit => _kit.Get(((ICodeApiServiceInternal)this).GetKit<TServiceKit>);
    private readonly GetOnce<TServiceKit> _kit = new();

}