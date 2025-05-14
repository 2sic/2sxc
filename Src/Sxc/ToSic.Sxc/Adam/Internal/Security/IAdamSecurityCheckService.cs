using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Adam.Internal;

public interface IAdamSecurityCheckService
{
    bool SiteAllowsExtension(string fileName);
    bool CanEditFolder(Eav.Apps.Assets.IAsset item);

    /// <summary>
    /// This will check if the field-definition grants additional rights
    /// Should only be called if the user doesn't have full edit-rights
    /// </summary>
    bool FieldPermissionOk(List<Grants> requiredGrant);

    internal IAdamSecurityCheckService Init(AdamContext adamContext, bool usePortalRoot);
    bool ExtensionIsOk(string fileName, out HttpExceptionAbstraction preparedException);

    /// <summary>
    /// Returns true if user isn't restricted, or if the restricted user is accessing a draft item
    /// </summary>
    bool UserIsNotRestrictedOrItemIsDraft(Guid guid, out HttpExceptionAbstraction exp);

    bool FileTypeIsOkForThisField(out HttpExceptionAbstraction preparedException);
    bool UserIsPermittedOnField(List<Grants> requiredPermissions, out HttpExceptionAbstraction preparedException);
    bool MustThrowIfAccessingRootButNotAllowed(bool usePortalRoot, out HttpExceptionAbstraction preparedException);
    bool UserIsRestricted { get; }
    bool SuperUserOrAccessingItemFolder(string path, out HttpExceptionAbstraction preparedException);
}