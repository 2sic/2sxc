using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class SxcContextResolver(
    LazySvc<AppIdResolver> appIdResolverLazy,
    Generator<IContextOfSite> siteCtxGenerator,
    Generator<IContextOfApp> appCtxGenerator,
    Lazy<IFeaturesService> featuresService)
    : ContextResolver(siteCtxGenerator, appCtxGenerator, "Sxc.CtxRes", connect: [appIdResolverLazy, siteCtxGenerator, appCtxGenerator, featuresService]), ISxcContextResolver
{

    /// <summary>
    /// Get the best possible context which can give us insights about the user permissions.
    ///
    /// TODO: WIP - requires that if an app is to be used, it was accessed before - not yet perfect...
    /// </summary>
    /// <returns></returns>
    public IContextOfUserPermissions UserPermissions() => _ctxUserPerm.Get(() => BlockContextOrNull() ?? AppOrNull() ?? Site());
    private readonly GetOnce<IContextOfUserPermissions> _ctxUserPerm = new();

    public IContextOfApp SetAppOrNull(string nameOrPath)
    {
        if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
        var zoneId = Site().Site.ZoneId;
        var appId = appIdResolverLazy.Value.GetAppIdFromPath(zoneId, nameOrPath, false);
        return appId <= Eav.Constants.AppIdEmpty
            ? null
            : SetApp(new AppIdentity(zoneId, appId));
    }

    #region Blocks

    public void AttachBlock(IBlock block)
    {
        _block = block;
        AppContextFromAppOrBlock = _block?.Context;
    }
    private IBlock _block;

    public IBlock BlockOrNull() => _block;

    public IBlock BlockRequired() => BlockOrNull()
                                     ?? throw new("Block required but missing. It was not attached");

    public IContextOfBlock BlockContextRequired() => BlockContextOrNull()
                                                     ?? throw new("Block context required but not known. It was not attached.");

    public IContextOfBlock BlockContextOrNull() => _block?.Context;


    #endregion
}