using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityBackend: WebApiBackendBase<EntityBackend>
    {
        public EntityBackend(IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Entity") { }

        // New feature in 11.03 - Usage Statistics

        public dynamic Usage(IContextOfBlock context, IApp app, Guid guid)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(context, app, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var appData = permCheck.App.Data;
            var item = appData.Immutable.One(guid);
            var relationships = item.Relationships.AllRelationships;
            // var result = relationships.Select(r => new EntityInRelationDto(r.))
            // todo: don't forget Metadata relationships
            return null;
        }



    }
}
