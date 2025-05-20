using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Manager;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Adam.Work.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AdamWorkBase(AdamWorkBase.MyServices services, string logName)
    : ServiceBase<AdamWorkBase.MyServices>(services, logName), IAdamWork
{
    #region MyServices / Init

    public class MyServices(LazySvc<AdamContext> adamContext, ISxcAppContextResolver ctxResolver, AdamGenericHelper adamGenericHelper)
        : MyServicesBase(connect: [adamContext, ctxResolver, adamGenericHelper])
    {
        public LazySvc<AdamContext> AdamContext { get; } = adamContext;
        public ISxcAppContextResolver CtxResolver { get; } = ctxResolver;
        public AdamGenericHelper AdamGenericHelper { get; } = adamGenericHelper;
    }

    public void Setup(AdamWorkOptions options)
    {
        var o = options;
        var context = options.AppId > 0
            ? Services.CtxResolver.GetExistingAppOrSet(options.AppId)
            : Services.CtxResolver.AppNameRouteBlock(null);
        var l = Log.Fn($"app: {context.AppReader.Show()}, type: {o.ContentType}, itemGuid: {o.ItemGuid}, field: {o.Field}, portalRoot: {o.UsePortalRoot}");
        AdamContext.Init(context, o.ContentType, o.Field, o.ItemGuid, o.UsePortalRoot);
        l.Done();
    }

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
    protected void VerifySecurityAndStructure(IFolder parentFolder, ToSic.Eav.Apps.Assets.IAsset target, string errPrefix)
    {
        // In case the primary file system is used (usePortalRoot) then also check higher permissions
        if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(target)) 
            throw HttpException.PermissionDenied(errPrefix + " - permission denied");

        if (!AdamContext.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
            throw exp;

        //if (!EqualityComparer<TFolderId>.Default.Equals(((IAssetWithParentSysId<TFolderId>)target).ParentSysId,
        //        ((IAssetSysId<TFolderId>)parentFolder).SysId))
        if (!Services.AdamGenericHelper.AssetIsChildOfFolder(parentFolder, target))
            throw HttpException.BadRequest(errPrefix + " - not found in folder");
    }

}