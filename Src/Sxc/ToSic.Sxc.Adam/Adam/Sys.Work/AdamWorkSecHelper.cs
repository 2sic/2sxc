using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class AdamWorkSecHelper
{

    /// <summary>
    /// Validate that user has write permissions for folder.
    /// In case the primary file system is used (usePortalRoot) then also check higher permissions
    /// </summary>
    //[AssertionMethod]
    internal static void VerifySecurityAndStructure(AdamContext adamCtx, AdamGenericHelper adamGenericHelper, IFolder? parentFolder, ToSic.Eav.Apps.Assets.IAsset target, string errPrefix)
    {
        // In case the primary file system is used (usePortalRoot) then also check higher permissions
        if (adamCtx.UseSiteRoot && !adamCtx.Security.CanEditFolder(target)) 
            throw HttpException.PermissionDenied(errPrefix + " - permission denied");

        if (adamCtx.Security.UserIsRestrictedAndAccessingItemOutsideOfFolder(target.Path, out var exp))
            throw exp;

        //if (!EqualityComparer<TFolderId>.Default.Equals(((IAssetWithParentSysId<TFolderId>)target).ParentSysId,
        //        ((IAssetSysId<TFolderId>)parentFolder).SysId))
        if (parentFolder == null || !adamGenericHelper.AssetIsChildOfFolder(parentFolder, target))
            throw HttpException.BadRequest(errPrefix + " - not found in folder");
    }

}
