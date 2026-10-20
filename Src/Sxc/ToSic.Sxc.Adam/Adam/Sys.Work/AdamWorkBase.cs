using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamWorkBase(AdamWorkBase.Dependencies services, string logName)
    : ServiceBase<AdamWorkBase.Dependencies>(services, logName), IAdamWork
{
    #region MyServices / Init

    public record Dependencies(
        LazySvc<AdamContext> AdamContext,
        IContextOfAppFactory AppCtxFactory,
        AdamGenericHelper AdamGenericHelper)
        : DependenciesBase(connect: [AdamContext, AppCtxFactory, AdamGenericHelper]);

    public void Setup(AdamWorkOptions options)
    {
        var context = Services.AppCtxFactory.Create(options.AppId);

        // 2026-10-20 2dm: Believe that the AppId should always have a valid value, so no magic find-it-yourself
        // Turned this off for now, keep till early 2027 then clean up.
        // If something does explode, try to ensure that the calling path sets a proper AppId _before_ calling this
        //var context = options.AppId > 0
        //    ? Services.CtxService.GetExistingAppOrSet(options.AppId)
        //    : Services.CtxService.SetAppFromNameOrUrlParamsOrGetExistingBlock(null);

        var o = options;
        var l = Log.Fn($"app: {context.AppReaderRequired.Show()}, type: {o.ContentType}, itemGuid: {o.ItemGuid}, field: {o.Field}, portalRoot: {o.UsePortalRoot}");
        AdamContext.Init(context, o.ContentType, o.Field, o.ItemGuid, o.UsePortalRoot);
        l.Done();
    }

    [field: AllowNull, MaybeNull]
    public AdamContext AdamContext => field ??= Services.AdamContext.Value;

    #endregion

    /// <summary>
    /// Validate that user has write permissions for folder.
    /// In case the primary file system is used (usePortalRoot) then also check higher permissions
    /// </summary>
    /// <param name="parentFolder"></param>
    /// <param name="target"></param>
    /// <param name="errPrefix"></param>
    //[AssertionMethod]
    protected void VerifySecurityAndStructure(IFolder? parentFolder, ToSic.Eav.Apps.Assets.IAsset target, string errPrefix)
    {
        // In case the primary file system is used (usePortalRoot) then also check higher permissions
        if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(target)) 
            throw HttpException.PermissionDenied(errPrefix + " - permission denied");

        if (AdamContext.Security.UserIsRestrictedAndAccessingItemOutsideOfFolder(target.Path, out var exp))
            throw exp;

        //if (!EqualityComparer<TFolderId>.Default.Equals(((IAssetWithParentSysId<TFolderId>)target).ParentSysId,
        //        ((IAssetSysId<TFolderId>)parentFolder).SysId))
        if (parentFolder == null || !Services.AdamGenericHelper.AssetIsChildOfFolder(parentFolder, target))
            throw HttpException.BadRequest(errPrefix + " - not found in folder");
    }

}