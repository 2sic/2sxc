using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Dnn.WebApi.Sys
{
    public class InstallController : DnnApiControllerWithFixes<InstallControllerReal<HttpResponseMessage>>, IInstallController<HttpResponseMessage>
    {
        public InstallController() : base(InstallControllerReal<HttpResponseMessage>.LogSuffix) { }

        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup => "web-api.install";


        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool Resume() => Real.Resume();


        ///// <inheritdoc />
        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        //public HttpResponseMessage RemoteWizardUrl(bool isContentApp)
        //{
        //    PrepareResponseMaker();
        //    return Real.RemoteWizardUrl(isContentApp,
        //        ((DnnModule) GetService<IModule>()).Init(Request.FindModuleInfo(), Log));
        //}

        private void PrepareResponseMaker()
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (ResponseMakerNetFramework)GetService<ResponseMaker<HttpResponseMessage>>();
            responseMaker.Init(this);
        }

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public InstallAppsDto InstallSettings(bool isContentApp) 
            => Real.InstallSettings(isContentApp, ((DnnModule) GetService<IModule>()).Init(Request.FindModuleInfo(), Log));


        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken] // now activate this, as it's post now, previously not, because this is a GET and can't include the RVT
        public HttpResponseMessage RemotePackage(string packageUrl)
        {
            PreventServerTimeout300();
            PrepareResponseMaker();
            return Real.RemotePackage(packageUrl, ((DnnModule)GetService<IModule>()).Init(ActiveModule, Log));
        }
    }
}
