using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi.Cms
{
    public class IdentifierHelper: HasLog<IdentifierHelper>
    {
        public IdentifierHelper(CmsRuntime cmsRuntime, IAppStates appStates) : base("Bck.IdHlpr")
        {
            _cmsRuntime = cmsRuntime;
            _appStates = appStates;
        }

        private readonly CmsRuntime _cmsRuntime;
        private readonly IAppStates _appStates;

        internal ItemIdentifier ResolveItemIdOfGroup(int appId, ItemIdentifier item, ILog log)
        {
            if (item.Group == null) return item;
            var cms = _cmsRuntime.Init(_appStates.Identity(null, appId), true, log);

            var contentGroup = cms.Blocks.GetBlockConfig(item.Group.Guid);
            var part = contentGroup[item.Group.Part];
            item.EntityId = part[item.ListIndex()].EntityId;
            return item;
        }

    }
}
