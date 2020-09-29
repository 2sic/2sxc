using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class HistoryController : SxcApiControllerBase, IHistoryController
    {
        protected override string HistoryLogName => "Api.History";

        /// <summary>
        /// Used to be POST Entities/History
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item) 
            => new AppManager(appId, Log).Entities.VersionHistory(EntityBackend.ResolveItemIdOfGroup(appId, item, Log).EntityId);

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item)
        {
            new AppManager(appId, Log).Entities.VersionRestore(EntityBackend.ResolveItemIdOfGroup(appId, item, Log).EntityId, changeId);
            return true;
        }

    }
}
