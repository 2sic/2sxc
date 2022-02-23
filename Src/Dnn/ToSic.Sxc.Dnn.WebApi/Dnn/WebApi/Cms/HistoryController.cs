using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class HistoryController : SxcApiControllerBase<DummyControllerReal>, IHistoryController
    {
        public HistoryController() : base("History") { }

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item) 
            => GetService<AppManager>().Init(appId, Log).Entities
                .VersionHistory(GetService<IdentifierHelper>().Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item)
        {
            GetService<AppManager>().Init(appId, Log).Entities
                .VersionRestore(GetService<IdentifierHelper>().Init(Log).ResolveItemIdOfGroup(appId, item, Log).EntityId, changeId);
            return true;
        }

    }
}
