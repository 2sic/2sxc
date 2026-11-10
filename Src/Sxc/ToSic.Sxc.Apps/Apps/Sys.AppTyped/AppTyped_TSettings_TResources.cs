using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Services.DataService.Sys;

namespace ToSic.Sxc.Apps.Sys.AppTyped;

internal class AppTyped<TSettings, TResources>(LazySvc<GlobalPaths> globalPaths, Generator<GetQueryMs<TypedQuery>, DataSourceOptionsMs> queryBuilder)
    : AppTyped(globalPaths, queryBuilder), IAppTyped<TSettings, TResources>
    where TSettings : class, IModelFromData, new()
    where TResources : class, IModelFromData, new()
{
    [field: AllowNull, MaybeNull]
    TSettings IAppTyped<TSettings, TResources>.Settings
        => field ??= Cdf.AsCustom<TSettings>(((IAppTyped)this).Settings)!;

    [field: AllowNull, MaybeNull]
    TResources IAppTyped<TSettings, TResources>.Resources
        => field ??= Cdf.AsCustom<TResources>(((IAppTyped)this).Resources)!;
    
}
