using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Caching;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Sxc.Sys.Configuration.SxcFeatures;

namespace ToSic.Sxc.Render.Sys.RenderBlock;
public class BlockCachingHelper(
    ISysFeaturesService featuresSvc,
    LazySvc<IAppsCatalog> appsCatalog,
    LazySvc<IAppReaderFactory> appReadersLazy,
    Generator<IAppPathsMicroSvc> appPathsLazy)
    : ServiceBase("Sxc.BlInHl", connect: [featuresSvc, appsCatalog, appReadersLazy, appPathsLazy])
{
    [field: AllowNull, MaybeNull]
    private LightSpeedConfigHelper LsConfigHelper => field ??= new(Log);

    internal bool PushAppDependenciesToRoot(IBlock? currentBlock)
    {
        var l = Log.Fn<bool>();
        if (currentBlock == null)
            return l.ReturnFalse("no block");

        var myAppId = currentBlock.AppId;
        // this is only relevant for the root builder, so we can skip it for child builders
        if (myAppId == 0)
            return l.ReturnFalse("no app");

        // make sure we have the app reader
        var appReader = currentBlock.Context.AppReaderOrNull;
        // if we don't have an app reader, we can't do anything
        if (appReader == null)
            return l.ReturnFalse("no app reader");

        // Cast to current object type to access internal APIs
        if (currentBlock.RootBlock is not { } rootBlock)
            return l.ReturnFalse("no root block");

        // add dependent appId only once
        var rootList = rootBlock.DependentApps;
        if (rootList.All(a => a.AppId != myAppId))
        {
            var decorator = LsConfigHelper.GetLightSpeedConfigOfApp(appReader);
            var appPathsToMonitor = featuresSvc.IsEnabled(LightSpeedOutputCacheAppFileChanges.NameId)
                ? AppPaths(appReader, currentBlock.Context.Site)
                : [];

            rootList.Add(new DependentApp
            {
                AppId = myAppId,
                IsSitePrimaryApp = false,
                LightSpeedDecorator = decorator,
                IsEnabled = decorator.IsEnabled,
                PathsToMonitor = appPathsToMonitor,
                CacheKeys = [MemoryCacheService.ExpandDependencyId(appReader.GetCache())],
            });
        }

        if (appReader.ZoneId >= 0)
        {
            l.A("dependentAppsStates add");
            var primary = appsCatalog.Value.PrimaryAppIdentity(appReader.ZoneId);
            var primaryReader = appReadersLazy.Value.Get(primary);
            rootList.Add(new DependentApp
            {
                AppId = primaryReader.AppId,
                IsSitePrimaryApp = true,
                LightSpeedDecorator = null,
                IsEnabled = true, // always enabled, since it's the primary app and shouldn't result in disabling LightSpeed
                PathsToMonitor = [],
                CacheKeys = [],
            });
        }

        return l.ReturnTrue("added");
    }


    /// <summary>
    /// Get physical paths for parent app and all dependent apps (portal and shared)
    /// </summary>
    /// <remarks>
    /// Note: The App Paths are only the apps in /2sxc (global and per portal)
    /// ADAM folders are not monitored
    /// </remarks>
    /// <returns>list of paths to monitor</returns>
    private IList<string> AppPaths(IAppReader appState, ISite site)
    {
        var paths = new List<string>();
        var appPaths = appPathsLazy.New().Get(appState, site);
        if (Directory.Exists(appPaths.PhysicalPath))
            paths.Add(appPaths.PhysicalPath);
        if (Directory.Exists(appPaths.PhysicalPathShared))
            paths.Add(appPaths.PhysicalPathShared);

        return paths;
    }

}
