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
using ToSic.Sxc.Context;


namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityPickerBackend: WebApiBackendBase<EntityPickerBackend>
    {
        private readonly EntityPickerApi _entityPickerApi;
        private readonly IContextOfApp _context;

        public EntityPickerBackend(EntityPickerApi entityPickerApi, IContextOfApp context, IServiceProvider serviceProvider) : base(serviceProvider, "BE.EntPck")
        {
            _entityPickerApi = entityPickerApi;
            _context = context;
        }


        public IEnumerable<EntityForPickerDto> GetAvailableEntities(int appId, string[] items, string contentTypeName, int? dimensionId)
        {
            _context.ResetApp(appId);
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName)
                ? ServiceProvider.Build<MultiPermissionsApp>().Init(_context, _context.AppState, Log)
                : ServiceProvider.Build<MultiPermissionsTypes>().Init(_context, _context.AppState, contentTypeName, Log);
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
