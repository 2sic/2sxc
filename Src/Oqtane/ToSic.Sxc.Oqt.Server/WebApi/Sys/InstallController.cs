using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + "/" + AreaRoutes.Sys)]
    public class InstallController: OqtStatefulControllerBase<DummyControllerReal>
    {
        #region System Installation

        public InstallController(Lazy<IEnvironmentInstaller> envInstallerLazy, Lazy<ImportFromRemote> impFromRemoteLazy, Lazy<IUser> userLazy): base("Install")
        {
            _envInstallerLazy = envInstallerLazy;
            _impFromRemoteLazy = impFromRemoteLazy;
            _userLazy = userLazy;
        }
        private readonly Lazy<IEnvironmentInstaller> _envInstallerLazy;
        private readonly Lazy<ImportFromRemote> _impFromRemoteLazy;
        private readonly Lazy<IUser> _userLazy;

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
        public bool Resume() => _envInstallerLazy.Value.ResumeAbortedUpgrade();

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
        public IActionResult RemoteWizardUrl(bool isContentApp)
        {
            var result = _envInstallerLazy.Value.Init(Log)
                .GetAutoInstallPackagesUiUrl(
                    GetContext().Site,
                    GetContext().Module,
                    isContentApp);
            return Json(result);
        }

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
            // Ensure that Hot Reload is not enabled or try to disable it.
            HotReloadEnabledCheck.Check();

            PreventServerTimeout300();

            var container = GetContext().Module;
            var isApp = !container.IsContent;

            Log.Add("install package:" + packageUrl);

            var block = container.BlockIdentifier;
            var result = _impFromRemoteLazy.Value.Init(_userLazy.Value, Log)
                .InstallPackage(block.ZoneId, block.AppId, isApp, packageUrl);

            Log.Add("install completed with success:" + result.Item1);

            return (result.Item1 ? Ok(new { result.Item1, result.Item2 }) : Problem());
        }

        #endregion
    }
}
