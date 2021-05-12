using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ApiExplorer;
using ToSic.Sxc.WebApi.Plumbing;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]
    
    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]

    public class ApiExplorerController : OqtStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.Explorer";

        [HttpGet]
        public IActionResult Inspect(string path)
        {
            var wrapLog = Log.Call<IActionResult>();

            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)ServiceProvider.Build<ResponseMaker>() ;
            responseMaker.Init(this);
            
            var backend = ServiceProvider.Build<ApiExplorerBackend>();
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
            var siteStateInitializer = ServiceProvider.Build<SiteStateInitializer>();
            var siteId = siteStateInitializer.InitializedState.Alias.SiteId;

            var oqtAppFolder = ServiceProvider.Build<OqtAppFolder>();
            var appFolder = oqtAppFolder.GetAppFolder();

            return OqtServerPaths.GetAppApiPath(siteId, appFolder, path);
        }

        private string GetFullPath(string pathFromRoot)
        {
            var oqtServerPaths = ServiceProvider.Build<IServerPaths>();
            return oqtServerPaths.FullContentPath(pathFromRoot);
        }

        private static string GetDllName(string controllerVirtualPath, string apiFile)
        {
            var controllerFolder = controllerVirtualPath.Substring(0, controllerVirtualPath.LastIndexOf(@"\"));
            return AppApiDynamicRouteValueTransformer.GetDllName(controllerFolder, apiFile);
        }
    }

}
