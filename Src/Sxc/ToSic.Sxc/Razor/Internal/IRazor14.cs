using Custom.Hybrid;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Razor.Internal;

[PrivateApi("not sure yet if this will stay in Hybrid or go to Web.Razor or something, so keep it private for now")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IRazor14<out TModel, out TServiceKit>: IRazor, IDynamicCode14<TModel, TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
    /// <summary>
    /// Dynamic object containing parameters. So in Dnn it contains the PageData, in Oqtane it contains the Model
    /// </summary>
    /// <remarks>
    /// New in v12
    /// </remarks>
    [PrivateApi]
    dynamic DynamicModel { get; }
    
}