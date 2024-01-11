using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class DynamicCodeRoot<TModel, TServiceKit>(DynamicCodeRoot.MyServices services, string logPrefix)
    : DynamicCodeRoot(services, logPrefix), IDynamicCodeRoot<TModel, TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
    //public TModel Model => default;

    public TServiceKit Kit => _kit.Get(GetService<TServiceKit>);
    private readonly GetOnce<TServiceKit> _kit = new();
}