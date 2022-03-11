using System;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityBackend: WebApiBackendBase<EntityBackend>
    {
        private readonly IContextResolver _ctxResolver;
        public EntityBackend(IServiceProvider serviceProvider, IContextResolver ctxResolver) : base(serviceProvider, "Bck.Entity")
        {
            _ctxResolver = ctxResolver.Init(Log);
        }

        // New feature in 11.03 - Usage Statistics

        public dynamic Usage(int appId, Guid guid)
        {
            var context = _ctxResolver.BlockOrApp(appId);
            var permCheck = GetService<MultiPermissionsApp>().Init(context, context.AppState, Log);
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
