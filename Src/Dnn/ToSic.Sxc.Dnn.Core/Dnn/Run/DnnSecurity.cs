using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.Internal;
using static ToSic.Sxc.Dnn.DnnSxcSettings;

namespace ToSic.Sxc.Dnn.Run;

// TODO: probably change this to use an interface so we can make it internal
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[PrivateApi]
public class DnnSecurity(LazySvc<RoleController> roleController) : ServiceBase("dnnSec", connect: [roleController])
{
    /// <summary>
    /// Returns true if a DotNetNuke User Group "2sxc Designers" / "2sxcAdministrators" exists
    /// </summary>
    /// <param name="portalId"></param>
    /// <returns></returns>
    private bool PortalHasExplicitAdminGroups(int portalId)
        => DnnGroupsSxcAdmins.Any(grpName => PortalHasGroup(portalId, grpName));

    private bool IsExplicitAdmin(UserInfo user)
        => !IsNullOrAnonymous(user) && DnnGroupsSxcAdmins.Any(user.IsInRole);


    private bool PortalHasGroup(int portalId, string groupName)
        => roleController.Value.GetRoleByName(portalId, groupName) != null;

    private bool IsNullOrAnonymous(UserInfo user)
        => user == null || user.UserID == -1;


    internal EffectivePermissions UserMayAdminThis(UserInfo user)
    {
        // Null-Check
        if (IsNullOrAnonymous(user))
            return new(false);

        // Super always AppAdmin
        if (user.IsSuperUser)
            return new(true);

        var portal = PortalSettings.Current;

        // Skip the remaining tests if the portal isn't known
        if (portal == null)
            return new(false);

        var dnnPermissionProvider = PermissionProvider.Instance();
        if (!dnnPermissionProvider.IsPortalEditor())
            return new(false);

        // Non-SuperUsers must be PortalEditor AND in the group SxcAppAdmins
        var hasSpecialGroup = PortalHasExplicitAdminGroups(portal.PortalId);
        if (hasSpecialGroup && IsExplicitAdmin(user))
            return new(true);

        // If the special group doesn't exist, then the admin-state (which is true - since he got here) is valid
        // or if the special group exist, then all administrators will be treated as ContentAdmins (has fewer permissions).
        if (user.IsInRole(portal.AdministratorRoleName ?? DnnAdminRoleDefaultName))
            return new(isSiteAdmin: !hasSpecialGroup, isContentAdmin: true);

        // ... for "Content Managers"
        if (user.IsInRole(DnnContentManagers))
            return new(false, true);

        // ... for "Content Editors"
        if (user.IsInRole(DnnContentEditors))
            return new(false, false, true, true);

        // this should not happen
        return new(false);
    }


    private List<int> RoleList(UserInfo user, int? portalId = null)
        => IsNullOrAnonymous(user)
            ? []
            : user.Roles
                .Select(r => RoleController.Instance.GetRoleByName(portalId ?? user.PortalID, r))
                .Where(r => r != null)
                .Select(r => r.RoleID)
                .ToList();

    internal Guid UserGuid(UserInfo user)
        => Membership.GetUser(user.Username)?.ProviderUserKey as Guid? ?? Guid.Empty;

    internal string UserIdentityToken(UserInfo user)
        => IsNullOrAnonymous(user) ? SxcUserConstants.Anonymous : DnnConstants.UserTokenPrefix + user.UserID;

    internal CmsUserRaw CmsUserBuilder(UserInfo user, int siteId)
        => UserMayAdminThis(user).Map(adminInfo => new CmsUserRaw
            {
                Id = user.UserID,
                Guid = UserGuid(user),
                NameId = UserIdentityToken(user),
                Roles = RoleList(user, portalId: siteId),
                IsSystemAdmin = user.IsSuperUser,
                IsSiteAdmin = adminInfo.IsSiteAdmin,
                IsContentAdmin = adminInfo.IsContentAdmin,
                IsContentEditor = adminInfo.IsContentEditor,
                IsAnonymous = IsNullOrAnonymous(user),
                Created = user.CreatedOnDate,
                Modified = user.LastModifiedOnDate,
                //
                Username = user.Username,
                Email = user.Email,
                Name = user.DisplayName
            }
        );
}