using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Apps;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]

    public class ApiExplorerController : OqtStatefulControllerBase<DummyControllerReal>
    {
        public ApiExplorerController() : base("Explor") { }

        [HttpGet]
        public IActionResult Inspect(string path)
        {
            var wrapLog = Log.Call<IActionResult>();

            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>() ;
            responseMaker.Init(this);
            
            var backend = GetService<ApiExplorerBackend<IActionResult>>();
            if (backend.PreCheckAndCleanPath(ref path, out var error)) return error;

            try
            {
                var pathFromRoot = GetPathFromRoot(path);
                Log.Add($"Controller path from root: {pathFromRoot}");

                var apiFile = GetFullPath(pathFromRoot);
                if (!System.IO.File.Exists(apiFile))
                {
                    var msg = $"Error: can't find controller file: {pathFromRoot}";
                    return wrapLog(msg, responseMaker.InternalServerError(msg));
                }

                var dllName = GetDllName(pathFromRoot, apiFile);

                var assembly = new Compiler().Compile(apiFile, dllName);

                return wrapLog(null, backend.AnalyzeClassAndCreateDto(path, assembly));
            }
            catch (Exception exc)
            {
                return wrapLog($"Error: {exc.Message}.", responseMaker.InternalServerError(exc));
            }
        }

        private string GetPathFromRoot(string path)
        {
            var siteStateInitializer = GetService<SiteStateInitializer>();
            var siteId = siteStateInitializer.InitializedState.Alias.SiteId;

            var oqtAppFolder = GetService<AppFolder>();
            var appFolder = oqtAppFolder.GetAppFolder();

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
