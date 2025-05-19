using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Backend.Adam;

public record AdamWorkOptions(int AppId, string ContentType, Guid ItemGuid, string Field, bool UsePortalRoot);

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract partial class AdamWorkBase<TAdamWork, TFolderId, TFileId>(AdamWorkBase<TAdamWork, TFolderId, TFileId>.MyServices services, string logName)
    : ServiceBase<AdamWorkBase<TAdamWork, TFolderId, TFileId>.MyServices>(services, logName), IAdamWork
    where TAdamWork : AdamWorkBase<TAdamWork, TFolderId, TFileId>
{
    #region MyServices / Init

    public class MyServices(LazySvc<AdamContext<TFolderId, TFileId>> adamContext, ISxcAppContextResolver ctxResolver)
        : MyServicesBase(connect: [adamContext, ctxResolver])
    {
        public LazySvc<AdamContext<TFolderId, TFileId>> AdamContext { get; } = adamContext;
        public ISxcAppContextResolver CtxResolver { get; } = ctxResolver;
    }

    //public TAdamWork Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot)
    //{
    //    var context = appId > 0
    //        ? Services.CtxResolver.GetExistingAppOrSet(appId)
    //        : Services.CtxResolver.AppNameRouteBlock(null);
    //    var logCall = Log.Fn<TAdamWork>($"app: {context.AppReader.Show()}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
    //    AdamContext.Init(context, contentType, field, itemGuid, usePortalRoot, cdf: null);
    //    return logCall.Return(this as TAdamWork);
    //}

    void IAdamWork.SetupInternal(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot)
    {
        var context = appId > 0
            ? Services.CtxResolver.GetExistingAppOrSet(appId)
            : Services.CtxResolver.AppNameRouteBlock(null);
        var l = Log.Fn($"app: {context.AppReader.Show()}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
        AdamContext.Init(context, contentType, field, itemGuid, usePortalRoot, cdf: null);
        l.Done();

    }


    public AdamContext<TFolderId, TFileId> AdamContext => Services.AdamContext.Value;

    #endregion

    /// <summary>
    /// Validate that user has write permissions for folder.
    /// In case the primary file system is used (usePortalRoot) then also check higher permissions
    /// </summary>
    /// <param name="parentFolder"></param>
    /// <param name="target"></param>
    /// <param name="errPrefix"></param>
    //[AssertionMethod]
    protected void VerifySecurityAndStructure(Eav.Apps.Assets.Internal.Folder<TFolderId, TFileId> parentFolder, IAssetWithParentSysId<TFolderId> target, string errPrefix)
    {
        // In case the primary file system is used (usePortalRoot) then also check higher permissions
        if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(target)) 
            throw HttpException.PermissionDenied(errPrefix + " - permission denied");

        if (!AdamContext.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
            throw exp;

        if(!EqualityComparer<TFolderId>.Default.Equals(target.ParentSysId, parentFolder.SysId))
            throw HttpException.BadRequest(errPrefix + " - not found in folder");
    }

}