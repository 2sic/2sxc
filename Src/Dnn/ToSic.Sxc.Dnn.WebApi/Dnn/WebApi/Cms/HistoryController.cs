using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    public class HistoryController : SxcApiControllerBase<HistoryControllerReal>, IHistoryController
    {
        public HistoryController() : base(HistoryControllerReal.LogSuffix) { }

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item)
            => Real.Get(appId, item);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item) 
            => Real.Restore(appId, changeId, item);
    }
}
