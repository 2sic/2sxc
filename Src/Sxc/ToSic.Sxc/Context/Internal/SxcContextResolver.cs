using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class SxcContextResolver(
    LazySvc<AppIdResolver> appIdResolverLazy,
    Generator<IContextOfSite> contextOfSite,
    Generator<IContextOfApp> contextOfApp,
    Lazy<IFeaturesService> featuresService)
    : ContextResolver(contextOfSite, contextOfApp, "Sxc.CtxRes", connect: [appIdResolverLazy]), ISxcContextResolver
{
    #region Constructor / DI
        
    protected readonly LazySvc<AppIdResolver> AppIdResolver = appIdResolverLazy;

    #endregion


    /// <summary>
    /// Get the best possible context which can give us insights about the user permissions.
    ///
    /// TODO: WIP - requires that if an app is to be used, it was accessed before - not yet perfect...
    /// </summary>
    /// <returns></returns>
    public IContextOfUserPermissions UserPermissions() => _ctxUserPerm.Get(() => BlockContextOrNull() ?? LatestAppContext ?? Site());
    private readonly GetOnce<IContextOfUserPermissions> _ctxUserPerm = new();
}