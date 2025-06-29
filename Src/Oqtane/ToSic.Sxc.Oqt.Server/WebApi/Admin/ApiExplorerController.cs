﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Reflection;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Eav.WebApi.Sys.ApiExplorer;
using ToSic.Lib.DI;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Oqt.Server.Code.Internal;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Sys;
using ToSic.Sys.Utils;
using RealController = ToSic.Eav.WebApi.Sys.ApiExplorer.ApiExplorerControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ValidateAntiForgeryToken]
//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[Authorize(Roles = RoleNames.Admin)]

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ApiExplorerController() : OqtStatefulControllerBase(RealController.LogSuffix), IApiExplorerController
{
    private RealController Real => GetService<RealController>();
    private Generator<Compiler> Compiler => GetService<Generator<Compiler>>();

    [HttpGet]
    public IActionResult Inspect(string path)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.Inspect(path, GetCompiledAssembly);
    }

    private Assembly GetCompiledAssembly(string path)
    {
        // get path from root
        var siteId = GetService<SiteState>()?.Alias?.SiteId ?? GetService<AliasResolver>().Alias.SiteId;
        var appFolder = GetService<AppFolderLookupForWebApi>().GetAppFolder();
        var pathFromRoot = OqtServerPaths.GetAppApiPath(siteId, appFolder, path);

        // Figure out the current edition
        var blockOrNull = CtxHlp.BlockOptional;
        var edition = blockOrNull
            .NullOrGetWith(b => GetService<PolymorphConfigReader>().UseViewEditionOrGet(b));

        var spec = new HotBuildSpec(blockOrNull?.AppId ?? KnownAppsConstants.AppIdEmpty, edition: edition, appName: blockOrNull?.AppOrNull?.Name);

        Log.A($"Controller path from root: {pathFromRoot}");

        // get full path
        var oqtServerPaths = GetService<IServerPaths>();
        var apiFile = oqtServerPaths.FullContentPath(pathFromRoot);

        if (!System.IO.File.Exists(apiFile))
            throw new($"Error: can't find controller file: {pathFromRoot}");

        // get dll name
        var controllerFolder = pathFromRoot.Substring(0, pathFromRoot.LastIndexOf(@"\"));
        var dllName = AppApiDynamicRouteValueTransformer.GetDllName(controllerFolder, apiFile);

        return Compiler.New().Compile(apiFile, dllName, spec).Assembly;
    }

    [HttpGet]
    [JsonFormatter(Casing = Casing.Camel)]
    public AllApiFilesDto AppApiFiles(int appId) => Real.AppApiFiles(appId);
}