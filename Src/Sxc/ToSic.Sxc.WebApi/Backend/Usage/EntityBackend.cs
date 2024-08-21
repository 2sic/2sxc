using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Backend.Usage;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EntityBackend(
    ISxcContextResolver ctxResolver,
    Generator<MultiPermissionsApp> appPermissions)
    : ServiceBase("Bck.Entity", connect: [ctxResolver, appPermissions])
{
    // New feature in 11.03 - Usage Statistics

    public dynamic Usage(int appId, Guid guid)
    {
        var context = ctxResolver.GetBlockOrSetApp(appId);
        var permCheck = appPermissions.New().Init(context, context.AppReader);
        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
            throw HttpException.PermissionDenied(error);

        var item = context.AppReader.List.One(guid);
        // Note: this isn't proper yet, it's all relationships in the app, not just of this entity
        //var relationships = item.Relationships.AllRelationships;

        // var result = relationships.Select(r => new EntityInRelationDto(r.))
        // todo: don't forget Metadata relationships
        return null;
    }



}