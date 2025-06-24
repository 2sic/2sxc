using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Manager.Internal;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Security.Internal;

public interface IAdamSecurityCheckService
{
    bool SiteAllowsExtension(string fileName);
    bool CanEditFolder(Eav.Apps.Assets.IAsset? item);

    /// <summary>
    /// This will check if the field-definition grants additional rights
    /// Should only be called if the user doesn't have full edit-rights
    /// </summary>
    bool FieldPermissionOk(List<Grants> requiredGrant);

    internal IAdamSecurityCheckService Init(AdamContext adamContext, bool usePortalRoot);
    //bool ExtensionIsOk(string fileName, out HttpExceptionAbstraction preparedException);
    bool ExtensionIsNotOk(string fileName, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException);

    ///// <summary>
    ///// Returns true if user isn't restricted, or if the restricted user is accessing a draft item
    ///// </summary>
    //bool UserIsNotRestrictedOrItemIsDraft(Guid guid, [NotNullWhen(true)] out HttpExceptionAbstraction? exp);
    /// <summary>
    /// Returns true if user is restricted, or if the accessed item is missing or published (not draft).
    /// </summary>
    bool UserIsRestrictedOrItemIsNotDraft(Guid guid, [NotNullWhen(true)] out HttpExceptionAbstraction? exp);

    //bool FileTypeIsOkForThisField(out HttpExceptionAbstraction preparedException);
    bool FieldDoesNotSupportFiles([NotNullWhen(true)] out HttpExceptionAbstraction? preparedException);

    //bool UserIsPermittedOnField(List<Grants> requiredPermissions, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException);

    bool UserNotPermittedOnField(List<Grants> requiredPermissions, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException);

    bool MustThrowIfAccessingRootButNotAllowed(bool usePortalRoot, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException);
    bool UserIsRestricted { get; }
    //bool SuperUserOrAccessingItemFolder(string path, out HttpExceptionAbstraction? preparedException);
    bool UserIsRestrictedAndAccessingItemOutsideOfFolder([NotNullWhen(false)]  string? path, [NotNullWhen(true)] out HttpExceptionAbstraction? preparedException);
}