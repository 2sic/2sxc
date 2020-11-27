using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityPickerBackend: WebApiBackendBase<EntityPickerBackend>
    {
        private readonly EntityPickerApi _entityPickerApi;

        public EntityPickerBackend(EntityPickerApi entityPickerApi, IServiceProvider serviceProvider) : base(serviceProvider, "BE.EntPck")
        {
            _entityPickerApi = entityPickerApi;
        }


        public IEnumerable<EntityForPickerDto> GetAvailableEntities(IContextOfBlock ctx, int appId, string[] items, string contentTypeName, int? dimensionId)
        {
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName)
                ? ServiceProvider.Build<MultiPermissionsApp>().Init(ctx, GetApp(appId, null), Log)
                : ServiceProvider.Build<MultiPermissionsTypes>().Init(ctx, GetApp(appId, null), contentTypeName, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // maybe in the future, ATM not relevant
            var withDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);

            return _entityPickerApi
                .Init(Log)
                .GetAvailableEntities(appId, items, contentTypeName, withDrafts, dimensionId);
        }
    }
}
