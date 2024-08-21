using JetBrains.Annotations;
using ToSic.Eav.Apps.Assets.Internal;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Backend.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract partial class AdamTransactionBase<T, TFolderId, TFileId>(
    AdamTransactionBase<T, TFolderId, TFileId>.MyServices services,
    string logName)
    : ServiceBase<AdamTransactionBase<T, TFolderId, TFileId>.MyServices>(services, logName), IAdamTransactionBase
    where T : AdamTransactionBase<T, TFolderId, TFileId>
{

    #region Constructor / DI
    public class MyServices : MyServicesBase
    {
        public LazySvc<AdamContext<TFolderId, TFileId>> AdamState { get; }
        public ISxcContextResolver CtxResolver { get; }
        public Generator<AdamItemDtoMaker<TFolderId, TFileId>> AdamDtoMaker { get; }

        public MyServices(
            Generator<AdamItemDtoMaker<TFolderId, TFileId>> adamDtoMaker,
            LazySvc<AdamContext<TFolderId, TFileId>> adamState,
            ISxcContextResolver ctxResolver)
        {
            ConnectLogs([
                AdamDtoMaker = adamDtoMaker,
                AdamState = adamState,
                CtxResolver = ctxResolver
            ]);
        }
    }

    public T Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot)
    {
        var context = appId > 0 ? Services.CtxResolver.GetBlockOrSetApp(appId) : Services.CtxResolver.AppNameRouteBlock(null);
        var logCall = Log.Fn<T>($"app: {context.AppReader.Show()}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
        AdamContext.Init(context, contentType, field, itemGuid, usePortalRoot, cdf: null);
        return logCall.Return(this as T);
    }

    void IAdamTransactionBase.Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot)
    {
        Init(appId, contentType, itemGuid, field, usePortalRoot);
    }


    protected AdamContext<TFolderId, TFileId> AdamContext => Services.AdamState.Value;

    #endregion

    /// <summary>
    /// Validate that user has write permissions for folder.
    /// In case the primary file system is used (usePortalRoot) then also check higher permissions
    /// </summary>
    /// <param name="parentFolder"></param>
    /// <param name="target"></param>
    /// <param name="errPrefix"></param>
    [AssertionMethod]
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