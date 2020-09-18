using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [ValidateAntiForgeryToken]
    public class InstallerController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.2sInst";

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        //[ValidateAntiForgeryToken] - never activate this, because this is a GET and can't include the RVT
        public HttpResponseMessage InstallPackage(string packageUrl)
        {
            PreventServerTimeout300();
            
            Log.Add("install package:" + packageUrl);
            var container = new DnnContainer().Init(ActiveModule, Log);
            var block = container.BlockIdentifier;

            var result = new ImportFromRemote().Init(new DnnUser(UserInfo), Log)
                .InstallPackage(block.ZoneId, block.AppId, ActiveModule.DesktopModule.ModuleName == "2sxc-app", packageUrl, Exceptions.LogException);

            Log.Add("install completed with success:" + result.Item1);
            return Request.CreateResponse(result.Item1 ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, new { result.Item1, result.Item2 });
        }

    }
}