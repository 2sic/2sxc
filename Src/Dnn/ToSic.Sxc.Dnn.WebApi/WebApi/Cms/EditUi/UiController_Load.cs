using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Dnn.WebApi.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class UiController
    {
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            return new EditLoadBackend()
                .Init(Log)
                .Load(GetBlock(),
                    new DnnContextBuilder(PortalSettings, ActiveModule, UserInfo), 
                    appId, items);
        }
    }
}
