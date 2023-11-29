using System;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Data;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EntityBackend: ServiceBase
{
    private readonly Generator<MultiPermissionsApp> _appPermissions;
    private readonly IContextResolver _ctxResolver;

    public EntityBackend(IContextResolver ctxResolver,
        Generator<MultiPermissionsApp> appPermissions) : base("Bck.Entity")
        => ConnectServices(
            _ctxResolver = ctxResolver,
            _appPermissions = appPermissions
        );

    // New feature in 11.03 - Usage Statistics

    public dynamic Usage(int appId, Guid guid)
    {
        var context = _ctxResolver.GetBlockOrSetApp(appId);
        var permCheck = _appPermissions.New().Init(context, context.AppStateReader);
        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
            throw HttpException.PermissionDenied(error);

        var item = context.AppStateReader.List.One(guid);
        // Note: this isn't proper yet, it's all relationships in the app, not just of this entity
        //var relationships = item.Relationships.AllRelationships;

        // var result = relationships.Select(r => new EntityInRelationDto(r.))
        // todo: don't forget Metadata relationships
        return null;
    }



}