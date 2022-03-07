using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    public class InstallController : DnnApiControllerWithFixes<InstallControllerReal<HttpResponseMessage>>
    {
        public InstallController() : base(InstallControllerReal<HttpResponseMessage>.LogSuffix) { }

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup => "web-api.install";

        #region System Installation

        /// <summary>
        /// Finish system installation which had somehow been interrupted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool Resume() => Real.Resume();

        #endregion


        #region App / Content Package Installation

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string RemoteWizardUrl(bool isContentApp) 
            => Real.RemoteWizardUrl(isContentApp, ((DnnModule) GetService<IModule>()).Init(Request.FindModuleInfo(), Log));


        /// <summary>
        /// Before this was GET Installer/InstallPackage
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken] // now activate this, as it's post now, previously not, because this is a GET and can't include the RVT
        public HttpResponseMessage RemotePackage(string packageUrl)
        {
            PreventServerTimeout300();

            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (ResponseMakerNetFramework)GetService<ResponseMaker<HttpResponseMessage>>();
            responseMaker.Init(this);

            return Real.RemotePackage(packageUrl, ((DnnModule)GetService<IModule>()).Init(ActiveModule, Log));
        }

        #endregion
    }
}
