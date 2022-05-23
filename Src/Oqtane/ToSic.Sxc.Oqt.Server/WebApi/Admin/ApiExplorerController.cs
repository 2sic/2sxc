using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Reflection;
using ToSic.Eav.Logging;
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

            return Real.Inspect(path, GetCompiledAssembly);
        }

        private Assembly GetCompiledAssembly(string path)
        {
            // get path from root
            var siteStateInitializer = GetService<SiteStateInitializer>();
            var siteId = siteStateInitializer.InitializedState.Alias.SiteId;
            var appFolder = GetService<AppFolder>().GetAppFolder();
            var pathFromRoot = OqtServerPaths.GetAppApiPath(siteId, appFolder, path);
            Log.A($"Controller path from root: {pathFromRoot}");

            // get full path
            var oqtServerPaths = GetService<IServerPaths>();
            var apiFile = oqtServerPaths.FullContentPath(pathFromRoot);

            if (!System.IO.File.Exists(apiFile))
                throw new Exception($"Error: can't find controller file: {pathFromRoot}");

            // get dll name
            var controllerFolder = pathFromRoot.Substring(0, pathFromRoot.LastIndexOf(@"\"));
            var dllName = AppApiDynamicRouteValueTransformer.GetDllName(controllerFolder, apiFile);

            return new Compiler().Compile(apiFile, dllName);
        }
    }

}
