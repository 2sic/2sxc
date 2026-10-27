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
    IContextBuilderService ctxBuilder,
    IContextCompleteService ctxService,
    IEditionService editionSvc,
    IServerPaths serverPaths)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.ApiCtlAsm", connect: [compiler, siteState, aliasResolver, ctxBuilder, ctxService, editionSvc, serverPaths]),
        IAppWebApiControllerAssemblyLoader
{
    public Assembly GetAssembly(string path)
    {
        var alias = siteState?.Alias ?? aliasResolver.Alias;
        var siteId = alias.SiteId;
        var tenantId = alias.TenantId;
        // Note 2026-10-18 2dm: #NoLookupSideEffects
        // the next line used to have a side effect of sharing the found app-context
        // This has been removed; it may break something, which would then need to be corrected "upstream", but not here
        var appFolder = ctxService.FindAppFolderFromScopeOrThrow(ctxBuilder);
        var pathFromRoot = OqtServerPaths.GetAppApiPath(tenantId, siteId, appFolder, path);

        var blockOrNull = ctxService.BlockOrNull;
        var edition = blockOrNull.NullOrGetWith(editionSvc.Edition);

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

        var result = compiler.New().Compile(apiFile, dllName, spec);
        return result.Assembly
            ?? throw new InvalidOperationException($"Error: can't compile controller file: {path}.\n{result.ErrorMessages}");
    }
}
