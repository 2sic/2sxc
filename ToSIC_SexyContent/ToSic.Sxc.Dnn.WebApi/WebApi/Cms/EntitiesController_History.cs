using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EntitiesController
    {

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> History(int appId, [FromBody] ItemIdentifier item)
        {
            ResolveItemIdOfGroup(appId, item);
            return new AppManager(appId, Log).Entities.VersionHistory(item.EntityId);
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item)
        {
            ResolveItemIdOfGroup(appId, item);
            new AppManager(appId, Log).Entities.VersionRestore(item.EntityId, changeId);
            return true;
        }

        private void ResolveItemIdOfGroup(int appId, ItemIdentifier item)
        {
            if (item.Group == null) return;
            var cms = new CmsRuntime(appId, Log, true);

            var contentGroup = cms.Blocks.GetBlockConfig(item.Group.Guid);
            var part = contentGroup[item.Group.Part];
            item.EntityId = part[item.ListIndex()].EntityId;
        }
    }
}
