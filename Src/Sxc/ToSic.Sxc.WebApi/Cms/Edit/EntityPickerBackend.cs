using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityPickerBackend: WebApiBackendBase<EntityPickerBackend>
    {
        #region DI Constructor

        public EntityPickerBackend(EntityPickerApi entityPickerApi,
            IContextResolver ctxResolver,
            GeneratorLog<MultiPermissionsApp> appPermissions,
            GeneratorLog<MultiPermissionsTypes> typePermissions,
            IServiceProvider serviceProvider) : base(serviceProvider, "BE.EntPck")
        {
            ConnectServices(
                _entityPickerApi = entityPickerApi,
                _ctxResolver = ctxResolver,
                _appPermissions = appPermissions,
                _typePermissions = typePermissions
            );
        }
        private readonly EntityPickerApi _entityPickerApi;
        private readonly IContextResolver _ctxResolver;
        private readonly GeneratorLog<MultiPermissionsApp> _appPermissions;
        private readonly GeneratorLog<MultiPermissionsTypes> _typePermissions;

        #endregion

        public IEnumerable<EntityForPickerDto> GetAvailableEntities(int appId, string[] items, string contentTypeName)
        {
            var context = _ctxResolver.BlockOrApp(appId);
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName)
                ? _appPermissions.New().Init(context, context.AppState)
                : _typePermissions.New().Init(context, context.AppState, contentTypeName);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // maybe in the future, ATM not relevant
            var withDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);

            return _entityPickerApi.GetAvailableEntities(appId, items, contentTypeName, withDrafts);
        }
    }
}
