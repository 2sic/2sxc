using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Context.Sys;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.Usage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EntityBackend(
    ISxcCurrentContextService ctxService,
    Generator<MultiPermissionsApp> appPermissions)
    : ServiceBase("Bck.Entity", connect: [ctxService, appPermissions])
{
    // New feature in 11.03 - Usage Statistics

    public object? Usage(int appId, Guid guid)
    {
        var context = ctxService.GetExistingAppOrSet(appId);
        var permCheck = appPermissions.New().Init(context, context.AppReaderRequired);
        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
            throw HttpException.PermissionDenied(error);

        var item = context.AppReaderRequired.List.One(guid);
        // Note: this isn't proper yet, it's all relationships in the app, not just of this entity
        //var relationships = item.Relationships.AllRelationships;

        // var result = relationships.Select(r => new EntityInRelationDto(r.))
        // todo: don't forget Metadata relationships
        return null;
    }



}