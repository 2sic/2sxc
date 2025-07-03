using System.Diagnostics.CodeAnalysis;
using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Apps.Internal;

internal class AppTyped<TSettings, TResources>(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager> queryManager)
    : AppTyped(globalPaths, queryManager), IAppTyped<TSettings, TResources>
    where TSettings : class, ICanWrapData, new()
    where TResources : class, ICanWrapData, new()
{
    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    [field: AllowNull, MaybeNull]
    TSettings IAppTyped<TSettings, TResources>.Settings
        => field ??= Cdf.AsCustom<TSettings>(((IAppTyped)this).Settings)!;

    [field: AllowNull, MaybeNull]
    TResources IAppTyped<TSettings, TResources>.Resources
        => field ??= Cdf.AsCustom<TResources>(((IAppTyped)this).Resources)!;
    
}
