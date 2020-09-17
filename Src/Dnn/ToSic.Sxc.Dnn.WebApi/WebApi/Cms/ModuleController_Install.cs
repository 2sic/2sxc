using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ModuleController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string RemoteInstallDialogUrl(string dialog, bool isContentApp)
        {
            if (dialog != "gettingstarted") throw new Exception("unknown dialog name: " + dialog);

            return Factory.Resolve<IEnvironmentInstaller>().Init(Log)
                .GetAutoInstallPackagesUiUrl(
                    new DnnTenant(PortalSettings),
                    new DnnContainer().Init(Request.FindModuleInfo(), Log), isContentApp, GetBlock().AppId);
        }

        /// <summary>
        /// Finish system installation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool FinishInstallation() => Factory.Resolve<IEnvironmentInstaller>().ResumeAbortedUpgrade();
    }
}
