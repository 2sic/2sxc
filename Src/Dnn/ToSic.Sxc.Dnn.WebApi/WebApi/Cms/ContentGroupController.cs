using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public partial class ContentGroupController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.ConGrp";

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public EntityInListDto Header(Guid guid) 
            => Factory.Resolve<ListsBackendBase>().Init(GetBlock().App, Log)
                .HeaderItem(guid);
    }
}