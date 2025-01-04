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
    TSettings IAppTyped<TSettings, TResources>.Settings
        => field ??= CodeApiSvc.Cdf.AsCustom<TSettings>(((IAppTyped)this).Settings);

    TResources IAppTyped<TSettings, TResources>.Resources
        => field ??= CodeApiSvc.Cdf.AsCustom<TResources>(((IAppTyped)this).Resources);
    
}
