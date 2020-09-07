using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    internal class EntityPickerBackend: WebApiBackendBase<EntityPickerBackend>
    {
        public EntityPickerBackend() : base("BE.EntPck")
        {

        }
        public IEnumerable<EntityForPickerDto> GetAvailableEntities(IInstanceContext ctx, int appId, string[] items, string contentTypeName, int? dimensionId)
        {
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName)
                ? new MultiPermissionsApp().Init(ctx, GetApp(appId, null), Log)
                : new MultiPermissionsTypes().Init(ctx, GetApp(appId, null), contentTypeName, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // maybe in the future, ATM not relevant
            var withDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);

            return new Eav.WebApi.EntityPickerApi(Log)
                .GetAvailableEntities(appId, items, contentTypeName, withDrafts, dimensionId);
        }
    }
}
