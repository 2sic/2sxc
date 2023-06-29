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

        private void PrepareResponseMaker()
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = SysHlp.GetResponseMaker();
            responseMaker.Init(this);
        }

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public InstallAppsDto InstallSettings(bool isContentApp) 
            => Real.InstallSettings(isContentApp, ((DnnModule)SysHlp.GetService<IModule>()).Init(Request.FindModuleInfo()));


        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken] // now activate this, as it's post now, previously not, because this is a GET and can't include the RVT
        public HttpResponseMessage RemotePackage(string packageUrl)
        {
            SysHlp.PreventServerTimeout300();
            PrepareResponseMaker();
            return Real.RemotePackage(packageUrl, ((DnnModule)SysHlp.GetService<IModule>()).Init(ActiveModule));
        }
    }
}
