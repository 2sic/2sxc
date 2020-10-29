using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.Oqt.Server.Controllers.Sys
{
    [Route(WebApiConstants.WebApiStateRoot + "/sys/install/[action]")]
    public class InstallController: SxcStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.Install";

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.install";


        #region System Installation

        public InstallController(StatefulControllerDependencies dependencies) : base(dependencies)
        { }

        /// <summary>
        /// Finish system installation which had somehow been interrupted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool Resume() => Eav.Factory.Resolve<IEnvironmentInstaller>().ResumeAbortedUpgrade();

        #endregion


        #region App / Content Package Installation

        /// <summary>
        /// Before this was GET Module/RemoteInstallDialogUrl 
        /// </summary>
        /// <param name="isContentApp"></param>
        /// <returns></returns>
        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IActionResult RemoteWizardUrl(bool isContentApp)
        {
            var result = Eav.Factory.Resolve<IEnvironmentInstaller>().Init(Log)
                .GetAutoInstallPackagesUiUrl(
                    GetContext().Tenant,
                    GetContext().Container,
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
        // [ValidateAntiForgeryToken] // now activate this, as it's post now, previously not, because this is a GET and can't include the RVT
        public IActionResult RemotePackage(string packageUrl)
        {
            PreventServerTimeout300();

            var oqtaneUser = GetContext().User;
            var container = GetContext().Container;
            bool isApp = !container.IsPrimary;

            Log.Add("install package:" + packageUrl);

            var block = container.BlockIdentifier;
            var result = Eav.Factory.Resolve<ImportFromRemote>().Init(oqtaneUser, Log)
                .InstallPackage(block.ZoneId, block.AppId, isApp, packageUrl);

            Log.Add("install completed with success:" + result.Item1);

            return (result.Item1 ? Ok(new { result.Item1, result.Item2 }) : Problem());
        }

        #endregion
    }
}
