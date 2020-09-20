using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    public partial class EntityController
    {

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> History(int appId, [FromBody] ItemIdentifier item) 
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
