using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Apps;

internal class AppTyped<TSettings, TResources>(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager> queryManager) : AppTyped(globalPaths, queryManager), IAppTyped<TSettings, TResources>
    where TSettings : class, ITypedItem, ITypedItemWrapper16, new()
    where TResources : class, ITypedItem, ITypedItemWrapper16, new()
{

    public new TSettings Settings => _settings ??= AsCustom<TSettings>(base.Settings);
    private TSettings _settings;

    public new TResources Resources => _resources ??= AsCustom<TResources>(base.Resources);
    private TResources _resources;

    private T AsCustom<T>(ICanBeEntity original) where T : class, ITypedItem, ITypedItemWrapper16, new()
        => CodeApiSvc.Cdf.AsCustom<T>(original);

}
