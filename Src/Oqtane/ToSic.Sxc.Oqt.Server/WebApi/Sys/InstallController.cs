using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    public class InstallController: OqtStatefulControllerBase<InstallControllerReal<IActionResult>>, IInstallController<IActionResult>
    {


        public InstallController(): base(InstallControllerReal<IActionResult>.LogSuffix) { }

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
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult RemoteWizardUrl(bool isContentApp)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>();
            responseMaker.Init(this);

            return Real.RemoteWizardUrl(isContentApp, GetContext().Module);
        }

        /// <inheritdoc />
        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult RemotePackage(string packageUrl)
        {
            HotReloadEnabledCheck.Check(); // Ensure that Hot Reload is not enabled or try to disable it.

            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>();
            responseMaker.Init(this);

            return Real.RemotePackage(packageUrl, GetContext().Module);
        }
    }
}
