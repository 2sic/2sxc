using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Code.Customizer;

public class Customizer(): ServiceForDynamicCode(SxcLogName + ".CdeCst")
{
    public IAppTyped<TSettings, TResources> App<TSettings, TResources>()
        where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
        where TResources : class, ITypedItem, ITypedItemWrapper16, new()
        => (_app ??= _CodeApiSvc.GetService<IAppTyped<TSettings, TResources>>(reuse: true)) as IAppTyped<TSettings, TResources>;
    private object _app;

    public object MyItem<TItem>()
        where TItem : class, ITypedItem, ITypedItemWrapper16, new()
    {
        var item = (_CodeApiSvc.Data as ContextData)?.MyItem?.FirstOrDefault();
        return _CodeApiSvc.Cdf.AsCustom<TItem>(item, default, false);
    }
}
