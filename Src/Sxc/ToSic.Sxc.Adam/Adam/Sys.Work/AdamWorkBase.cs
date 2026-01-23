using ToSic.Eav.Apps.Sys;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamWorkBase(AdamWorkBase.Dependencies services, string logName)
    : ServiceBase<AdamWorkBase.Dependencies>(services, logName), IAdamWork
{
    #region MyServices / Init

    public record Dependencies(
        LazySvc<AdamContext> AdamContext,
        ISxcAppCurrentContextService CtxService,
        AdamGenericHelper AdamGenericHelper)
        : DependenciesRecord(connect: [AdamContext, CtxService, AdamGenericHelper]);

    public void Setup(AdamWorkOptions options)
    {
        var o = options;
        var context = options.AppId > 0
            ? Services.CtxService.GetExistingAppOrSet(options.AppId)
            : Services.CtxService.AppNameRouteBlock(null);
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