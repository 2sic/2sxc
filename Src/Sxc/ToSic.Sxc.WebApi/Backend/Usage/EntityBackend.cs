// 2026-02-26 2dm - seems to be a partial implementation of relationships - never implemented, moving to SysData

//using ToSic.Eav.Apps.Sys.Permissions;
//using ToSic.Sys.Security.Permissions;

//namespace ToSic.Sxc.Backend.Usage;

//[ShowApiWhenReleased(ShowApiMode.Never)]
//public class EntityBackend(
//    ISxcCurrentContextService ctxService,
//    Generator<MultiPermissionsApp, MultiPermissionsApp.Options> appPermissions)
//    : ServiceBase("Bck.Entity", connect: [ctxService, appPermissions])
//{
//    // New feature in 11.03 - Usage Statistics

//    public object? Usage(int appId, Guid guid)
//    {
//        var context = ctxService.GetExistingAppOrSet(appId);
//        var permCheck = appPermissions.New(new(context, context.AppReaderRequired));
//        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var error))
//            throw HttpException.PermissionDenied(error);

//        var item = context.AppReaderRequired.List.GetOne(guid);
//        // Note: this isn't proper yet, it's all relationships in the app, not just of this entity
//        //var relationships = item.Relationships.AllRelationships;

//        // var result = relationships.Select(r => new EntityInRelationDto(r.))
//        // todo: don't forget Metadata relationships
//        return null;
//    }

//}