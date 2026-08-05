using System.Reflection;
using Oqtane.Shared;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.WebApi.Sys.ApiExplorer;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Code.Sys;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sxc.WebApi.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OqtAppWebApiControllerAssemblyLoader(
    Generator<Compiler> compiler,
    SiteState siteState,
    AliasResolver aliasResolver,
    AppFolderLookupForWebApi appFolderLookup,
    ISxcCurrentContextService ctxService,
    PolymorphConfigReader polymorphism,
    IServerPaths serverPaths)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.ApiCtlAsm", connect: [compiler, siteState, aliasResolver, appFolderLookup, ctxService, polymorphism, serverPaths]),
        IAppWebApiControllerAssemblyLoader
{
    public Assembly GetAssembly(string path)
    {
        var alias = siteState?.Alias ?? aliasResolver.Alias;
        var siteId = alias.SiteId;
        var tenantId = alias.TenantId;
        var appFolder = appFolderLookup.GetAppFolder();
        var pathFromRoot = OqtServerPaths.GetAppApiPath(tenantId, siteId, appFolder, path);

        var blockOrNull = ctxService.BlockOrNull();
        var edition = blockOrNull.NullOrGetWith(b => polymorphism.UseViewEditionOrGet(b));

        var runtimeKey = blockOrNull?.Context.AppReaderRequired?.Specs.CacheKey;
        var spec = new HotBuildSpec(blockOrNull?.AppId ?? KnownAppsConstants.AppIdEmpty,
            edition: edition,
            appName: blockOrNull?.AppOrNull?.Name,
            appCacheKey: runtimeKey);

        Log.A($"Controller path from root: {pathFromRoot}");

        var apiFile = serverPaths.FullContentPath(pathFromRoot);

        if (!File.Exists(apiFile))
            throw new($"Error: can't find controller file: {pathFromRoot}");

        var controllerFolder = pathFromRoot.Substring(0, pathFromRoot.LastIndexOf(@"\", StringComparison.Ordinal));
        var dllName = AppApiDynamicRouteValueTransformer.GetDllName(controllerFolder, apiFile);

        return compiler.New().Compile(apiFile, dllName, spec).Assembly;
    }
}
