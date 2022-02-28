using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    public class InstallController: OqtStatefulControllerBase<InstallControllerReal>
    {
        #region System Installation

        public InstallController(): base(InstallControllerReal.LogSuffix) { }

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.install";

        /// <summary>
        /// Finish system installation which had somehow been interrupted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        [Authorize(Roles = RoleNames.Host)]
        public bool Resume() => Real.Resume();

        #endregion


        #region App / Content Package Installation

        /// <summary>
        /// Before this was GET Module/RemoteInstallDialogUrl
        /// </summary>
        /// <param name="isContentApp"></param>
        /// <returns></returns>
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult RemoteWizardUrl(bool isContentApp) => Json(Real.RemoteWizardUrl(isContentApp, GetContext().Module));

        /// <summary>
        /// Before this was GET Installer/InstallPackage
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <returns></returns>
        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public IActionResult RemotePackage(string packageUrl)
        {
            HotReloadEnabledCheck.Check(); // Ensure that Hot Reload is not enabled or try to disable it.
            PreventServerTimeout300();
            var (success, messages) = Real.RemotePackage(packageUrl, GetContext().Module);
            return (success ? Ok(new {Item1 = success, Item2 = messages }) : Problem());
        }

        #endregion
    }
}
