using System;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EntitiesController
    {
        #region New feature in 11.03 - Usage Statitics

        public dynamic Usage(int appId, Guid guid)
        {
            var permCheck = new MultiPermissionsApp(BlockBuilder, appId, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
                throw exception;

            var appData = permCheck.App.Data;
            var item = appData.List.One(guid);
            var relationships = item.Relationships.AllRelationships;
            // var result = relationships.Select(r => new EntityInRelationDto(r.))
            // todo: don't forget Metadata relationships
            return null;
        }

        #endregion
    }
}
