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
public abstract class AdamWorkBase<TFolderId, TFileId>(AdamWorkBase<TFolderId, TFileId>.MyServices services, string logName)
    : ServiceBase<AdamWorkBase<TFolderId, TFileId>.MyServices>(services, logName), IAdamWork
{
    #region MyServices / Init

    public class MyServices(LazySvc<AdamContext<TFolderId, TFileId>> adamContext, ISxcAppContextResolver ctxResolver)
        : MyServicesBase(connect: [adamContext, ctxResolver])
    {
        public LazySvc<AdamContext<TFolderId, TFileId>> AdamContext { get; } = adamContext;
        public ISxcAppContextResolver CtxResolver { get; } = ctxResolver;
    }

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


    public AdamFolderFileSet<TFolderId, TFileId> ItemsInField(string subFolderName, bool autoCreate = false)
    {
        var l = Log.Fn<AdamFolderFileSet<TFolderId, TFileId>>($"Subfolder: {subFolderName}; AutoCreate: {autoCreate}");

        l.A("starting permissions checks");
        if (AdamContext.Security.UserIsRestricted && !AdamContext.Security.FieldPermissionOk(GrantSets.ReadSomething))
            return l.ReturnNull("user is restricted, and doesn't have permissions on field - return null");

        // check that if the user should only see drafts, he doesn't see items of published data
        if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out _))
            return l.ReturnNull("user is restricted (no read-published rights) and item is published - return null");

        l.A("first permission checks passed");

        // get root and at the same time auto-create the core folder in case it's missing (important)
        var root = AdamContext.AdamRoot.Folder(autoCreate);

        // if no root exists then quit now
        if (!autoCreate && root == null)
            return l.Return(new(null, [], []), "no folder");

        // try to see if we can get into the subfolder - will throw error if missing
        var currentFolder = AdamContext.AdamRoot.Folder(subFolderName, false);

        // ensure that it's super user, or the folder is really part of this item
        if (!AdamContext.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var ex))
        {
            l.A("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
            l.Ex(ex);
            throw ex;
        }

        var adamFolders = currentFolder.Folders
            .Cast<Sxc.Adam.Internal.Folder<TFolderId, TFileId>>()
            .Where(s => !EqualityComparer<TFolderId>.Default.Equals(s.SysId, currentFolder.SysId))
            .ToList();

        // Get/Cast Files
        var adamFiles = currentFolder.Files
            .Cast<Sxc.Adam.Internal.File<TFolderId, TFileId>>()
            .ToList();

        return l.Return(new(currentFolder, adamFolders, adamFiles), $"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count}");
    }
}