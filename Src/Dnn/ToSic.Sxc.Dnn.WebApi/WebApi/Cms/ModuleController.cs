using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {
        protected override string HistoryLogName => "Api.ModCnt";


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


    }
}