using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps;

internal class AppTyped<TSettings, TResources>(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager> queryManager)
    : AppTyped(globalPaths, queryManager), IAppTyped<TSettings, TResources>,
        IAppTyped
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{
    TSettings IAppTyped<TSettings, TResources>.Settings => _settings ??= AsCustom<TSettings>(((IAppTyped)this).Settings);
    private TSettings _settings;

    TResources IAppTyped<TSettings, TResources>.Resources => _resources ??= AsCustom<TResources>(((IAppTyped)this).Resources);
    private TResources _resources;

    private T AsCustom<T>(ICanBeEntity original) where T : class, ITypedItem, ITypedItemWrapper16, new()
        => CodeApiSvc.Cdf.AsCustom<T>(original);

}
