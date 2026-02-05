using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Apps.Sys.AppTyped;

internal class AppTyped<TSettings, TResources>(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager<TypedQuery>> queryManager)
    : AppTyped(globalPaths, queryManager), IAppTyped<TSettings, TResources>
    where TSettings : class, IModelOfData, new()
    where TResources : class, IModelOfData, new()
{
    [field: AllowNull, MaybeNull]
    TSettings IAppTyped<TSettings, TResources>.Settings
        => field ??= Cdf.AsCustom<TSettings>(((IAppTyped)this).Settings)!;

    [field: AllowNull, MaybeNull]
    TResources IAppTyped<TSettings, TResources>.Resources
        => field ??= Cdf.AsCustom<TResources>(((IAppTyped)this).Resources)!;
    
}
