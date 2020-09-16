using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.WebApi.Cms
{
	[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
	public partial class TemplateController : SxcApiControllerBase
	{
        protected override string HistoryLogName => "Api.TmpCnt";

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<object> GetAll(int appId)
            => new ViewsBackend().Init(new DnnTenant(PortalSettings), new DnnUser(UserInfo), Log)
                .GetAll(appId);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public PolymorphismDto Polymorphism(int appId) 
            => new PolymorphismBackend().Init(Log).Polymorphism(appId);


        [HttpGet, HttpDelete]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, int id)
            => new ViewsBackend().Init(new DnnTenant(PortalSettings), new DnnUser(UserInfo), Log)
                .Delete(appId, id);
    }
}