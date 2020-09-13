using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Dnn.WebApi.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public partial class UiController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.UiCont";

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => new EditLoadBackend().Init(Log)
                .Load(GetBlock(), new DnnContextBuilder(PortalSettings, ActiveModule, UserInfo), appId, items);

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] AllInOneDto package, int appId, bool partOfPage) 
            => new EditSaveBackend().Init(Log)
                .Save(GetBlock(), package, appId, partOfPage);
    }
}
