using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
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
    internal bool PortalHasExplicitAdminGroups(int portalId)
        => DnnGroupsSxcAdmins.Any(grpName => PortalHasGroup(portalId, grpName));

    internal bool IsExplicitAdmin(UserInfo user)
        => !IsAnonymous(user) && DnnGroupsSxcAdmins.Any(user.IsInRole);


    internal bool PortalHasGroup(int portalId, string groupName)
        => roleController.Value.GetRoleByName(portalId, groupName) != null;

    internal bool IsAnonymous(UserInfo user)
        => user == null || user.UserID == -1;


    internal AdminPermissions UserMayAdminThis(UserInfo user)
    {
        // Null-Check
        if (IsAnonymous(user)) return new(false);

        // Super always AppAdmin
        if (user.IsSuperUser) return new(true);

        var portal = PortalSettings.Current;

        // Skip the remaining tests if the portal isn't known
        if (portal == null) return new(false);

        // Non-SuperUsers must be Admin AND in the group SxcAppAdmins
        if (!user.IsInRole(portal.AdministratorRoleName ?? DnnAdminRoleDefaultName)) return new(false);

        var hasSpecialGroup = PortalHasExplicitAdminGroups(portal.PortalId);
        if (hasSpecialGroup && IsExplicitAdmin(user)) return new(true);

        // If the special group doesn't exist, then the admin-state (which is true - since he got here) is valid
        return new(true, !hasSpecialGroup);
    }


    internal List<int> RoleList(UserInfo user, int? portalId = null)
        => IsAnonymous(user)
            ? []
            : user.Roles
                .Select(r => RoleController.Instance.GetRoleByName(portalId ?? user.PortalID, r))
                .Where(r => r != null)
                .Select(r => r.RoleID)
                .ToList();

    internal Guid UserGuid(UserInfo user)
        => Membership.GetUser(user.Username)?.ProviderUserKey as Guid? ?? Guid.Empty;

    internal string UserIdentityToken(UserInfo user)
        => IsAnonymous(user) ? SxcUserConstants.Anonymous : DnnConstants.UserTokenPrefix + user.UserID;

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
                IsAnonymous = IsAnonymous(user),
                Created = user.CreatedOnDate,
                Modified = user.LastModifiedOnDate,
                //
                Username = user.Username,
                Email = user.Email,
                Name = user.DisplayName
            }
        );
}