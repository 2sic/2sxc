using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Usage;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
	[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	public class ViewController : SxcApiControllerBase
	{
        protected override string HistoryLogName => "Api.TmpCnt";

        [HttpGet]
        public IEnumerable<object> All(int appId)
            => new ViewsBackend().Init(new DnnTenant(PortalSettings), new DnnUser(UserInfo), Log)
                .GetAll(appId);

        [HttpGet]
        public PolymorphismDto Polymorphism(int appId) 
            => new PolymorphismBackend().Init(Log).Polymorphism(appId);


        [HttpGet, HttpDelete]
        public bool Delete(int appId, int id)
            => new ViewsBackend().Init(new DnnTenant(PortalSettings), new DnnUser(UserInfo), Log)
                .Delete(appId, id);

        [HttpGet]
        public IEnumerable<ViewDto> Usage(int appId, Guid guid)
            => new UsageBackend().Init(Log)
                .ViewUsage(GetContext(), appId, guid,
                    (views, blocks) =>
                    {
                        // create array with all 2sxc modules in this portal
                        var allMods = new Pages.Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
                        Log.Add($"Found {allMods.Count} modules");

                        return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
                    });

    }
}