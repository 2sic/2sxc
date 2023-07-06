using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using RealController = ToSic.Sxc.WebApi.Sys.InstallControllerReal<Microsoft.AspNetCore.Mvc.IActionResult>;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    public class InstallController: OqtStatefulControllerBase, IInstallController<IActionResult>
    {

        public InstallController(): base(RealController.LogSuffix) { }

        private RealController Real => GetService<RealController>();


        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.install";


        /// <inheritdoc />
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        [Authorize(Roles = RoleNames.Host)]
        public bool Resume() => Real.Resume();


        /// <inheritdoc />
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public InstallAppsDto InstallSettings(bool isContentApp)
            => Real.InstallSettings(isContentApp, SysHlp.BlockOptional.Context.Module);

        private void PrepareResponseMaker()
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>();
            responseMaker.Init(this);
        }

        /// <inheritdoc />
        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult RemotePackage(string packageUrl)
        {
            HotReloadEnabledCheck.Check(); // Ensure that Hot Reload is not enabled or try to disable it.
            PrepareResponseMaker();
            return Real.RemotePackage(packageUrl, SysHlp.BlockOptional?.Context.Module);
        }
    }
}
