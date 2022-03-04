using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Reflection;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]

    public class ApiExplorerController : OqtStatefulControllerBase<ApiExplorerControllerReal<IActionResult>>, IApiExplorerController<IActionResult>
    {
        public ApiExplorerController() : base(ApiExplorerControllerReal<IActionResult>.LogSuffix) { }

        [HttpGet]
        public IActionResult Inspect(string path)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>();
            responseMaker.Init(this);

            return Real.Inspect(path, GetAssembly);
        }

        private Assembly GetAssembly(string path)
        {
            var pathFromRoot = GetPathFromRoot(path);
            Log.Add($"Controller path from root: {pathFromRoot}");

            var apiFile = GetFullPath(pathFromRoot);

            if (!System.IO.File.Exists(apiFile))
                throw new Exception($"Error: can't find controller file: {pathFromRoot}");

            var dllName = GetDllName(pathFromRoot, apiFile);

            return new Compiler().Compile(apiFile, dllName);
        }

        private string GetPathFromRoot(string path)
        {
            var siteStateInitializer = GetService<SiteStateInitializer>();
            var siteId = siteStateInitializer.InitializedState.Alias.SiteId;

            var appFolder = GetService<AppFolder>().GetAppFolder();

            return OqtServerPaths.GetAppApiPath(siteId, appFolder, path);
        }

        private string GetFullPath(string pathFromRoot)
        {
            var oqtServerPaths = GetService<IServerPaths>();
            return oqtServerPaths.FullContentPath(pathFromRoot);
        }

        private static string GetDllName(string controllerVirtualPath, string apiFile)
        {
            var controllerFolder = controllerVirtualPath.Substring(0, controllerVirtualPath.LastIndexOf(@"\"));
            return AppApiDynamicRouteValueTransformer.GetDllName(controllerFolder, apiFile);
        }
    }

}
