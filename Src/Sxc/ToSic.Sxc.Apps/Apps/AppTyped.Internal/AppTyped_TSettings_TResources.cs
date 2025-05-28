using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Apps.Internal;

internal class AppTyped<TSettings, TResources>(LazySvc<GlobalPaths> globalPaths, LazySvc<QueryManager> queryManager)
    : AppTyped(globalPaths, queryManager), IAppTyped<TSettings, TResources>
    where TSettings : class, ICanWrapData, new()
    where TResources : class, ICanWrapData, new()
{
    private ICodeDataFactory Cdf => field ??= ExCtx.GetCdf();

    TSettings IAppTyped<TSettings, TResources>.Settings
        => field ??= Cdf.AsCustom<TSettings>(((IAppTyped)this).Settings);

    TResources IAppTyped<TSettings, TResources>.Resources
        => field ??= Cdf.AsCustom<TResources>(((IAppTyped)this).Resources);
    
}
