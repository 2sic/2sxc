using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.WebApi.Cms
{
    internal class EntityBackend: WebApiBackendBase<EntityBackend>
    {
        public EntityBackend() : base("Bck.Entity")
        {
        }

        #region New feature in 11.03 - Usage Statitics

        public dynamic Usage(IInstanceContext context, IApp app, Guid guid)
        {
            var permCheck = new MultiPermissionsApp().Init(context, app, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var appData = permCheck.App.Data;
            var item = appData.Immutable.One(guid);
            var relationships = item.Relationships.AllRelationships;
            // var result = relationships.Select(r => new EntityInRelationDto(r.))
            // todo: don't forget Metadata relationships
            return null;
        }

        #endregion

        internal static ItemIdentifier ResolveItemIdOfGroup(int appId, ItemIdentifier item, ILog log)
        {
            if (item.Group == null) return item;
            var cms = new CmsRuntime(appId, log, true);

            var contentGroup = cms.Blocks.GetBlockConfig(item.Group.Guid);
            var part = contentGroup[item.Group.Part];
            item.EntityId = part[item.ListIndex()].EntityId;
            return item;
        }

    }
}
