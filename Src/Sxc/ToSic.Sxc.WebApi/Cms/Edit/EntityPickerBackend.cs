using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EntityPickerBackend: ServiceBase
    {
        #region DI Constructor

        public EntityPickerBackend(EntityPickerApi entityPickerApi,
            IContextResolver ctxResolver,
            Generator<MultiPermissionsApp> appPermissions,
            Generator<MultiPermissionsTypes> typePermissions) : base("BE.EntPck")
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
        private readonly Generator<MultiPermissionsApp> _appPermissions;
        private readonly Generator<MultiPermissionsTypes> _typePermissions;

        #endregion

        // 2dm 2023-01-22 #maybeSupportIncludeParentApps
        public IEnumerable<EntityForPickerDto> GetForEntityPicker(int appId, string[] items, string contentTypeName/*, bool includeParentApps*/) => Log.Func(() =>
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

            return _entityPickerApi.GetForEntityPicker(appId, items, contentTypeName, withDrafts/*, includeParentApps*/);
        });
    }
}
