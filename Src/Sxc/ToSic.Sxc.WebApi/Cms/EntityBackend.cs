using System;
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

        public dynamic Usage(IContextOfApp context, Guid guid)
        {
            var permCheck = ServiceProvider.Build<MultiPermissionsApp>().Init(context, context.AppState, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            var item = context.AppState.List.One(guid);
            var relationships = item.Relationships.AllRelationships;
            // var result = relationships.Select(r => new EntityInRelationDto(r.))
            // todo: don't forget Metadata relationships
            return null;
        }



    }
}
