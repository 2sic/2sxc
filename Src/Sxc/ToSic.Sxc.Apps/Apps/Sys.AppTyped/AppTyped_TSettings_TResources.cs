using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Services.DataService.Sys;

namespace ToSic.Sxc.Apps.Sys.AppTyped;

/// <summary>
/// Implementation of the typed App object on custom code, where the Settings and Resources are also typed according to the generated code.
/// </summary>
/// <typeparam name="TSettings">Custom class/type of the settings.</typeparam>
/// <typeparam name="TResources">Custom class/type of the resources</typeparam>
/// <param name="globalPaths"></param>
/// <param name="queryBuilder"></param>
internal class AppTyped<TSettings, TResources>(Ctx<IAppForCode> app, Ctx<ICodeDataFactory> cdf, LazySvc<GlobalPaths> globalPaths, Generator<GetQueryMs<TypedQuery>, DataSourceOptionsMs> queryBuilder)
    : AppTyped(app, cdf, globalPaths, queryBuilder), IAppTyped<TSettings, TResources>
    where TSettings : class, IModelFromData, new()
    where TResources : class, IModelFromData, new()
{
    /// <summary>
    /// Explicit implementation of the settings, when accessed through the proper interface (this is the default case).
    /// </summary>
    [field: AllowNull, MaybeNull]
    TSettings IAppTyped<TSettings, TResources>.Settings
        => field ??= Cdf.AsCustom<TSettings>(((IAppTyped)this).Settings)!;

    /// <summary>
    /// Explicit implementation of the resources, when accessed through the proper interface (this is the default case).
    /// </summary>
    [field: AllowNull, MaybeNull]
    TResources IAppTyped<TSettings, TResources>.Resources
        => field ??= Cdf.AsCustom<TResources>(((IAppTyped)this).Resources)!;
    
}
