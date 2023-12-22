using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Raw;
using static ToSic.Sxc.Dnn.DnnSxcSettings;

namespace ToSic.Sxc.Dnn.Run;

// TODO: probably change this to use an interface so we can make it internal
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[PrivateApi]
public class DnnSecurity : ServiceBase
{
    private readonly LazySvc<RoleController> _roleController;

    public DnnSecurity(LazySvc<RoleController> roleController) : base("dnnSec")
    {
        ConnectServices(
            _roleController = roleController
        );
    }

    /// <summary>
    /// Returns true if a DotNetNuke User Group "2sxc Designers" exists
    /// </summary>
    /// <param name="portalId"></param>
    /// <returns></returns>
    internal bool PortalHasExplicitAdminGroups(int portalId)
        => PortalHasGroup(portalId, DnnGroupSxcDesigners) || PortalHasGroup(portalId, DnnGroupSxcAdmins);

    internal bool IsExplicitAdmin(UserInfo user)
        => !IsAnonymous(user) && (user.IsInRole(DnnGroupSxcDesigners) || user.IsInRole(DnnGroupSxcAdmins));


    internal bool PortalHasGroup(int portalId, string groupName) => _roleController.Value.GetRoleByName(portalId, groupName) != null;

    internal bool IsAnonymous(UserInfo user) => user == null || user.UserID == -1;

    internal class DnnSiteAdminPermissions
    {
        public bool IsContentAdmin;
        public bool IsSiteAdmin;

        public DnnSiteAdminPermissions(bool both): this(both, both) { }

        public DnnSiteAdminPermissions(bool isContentAdmin, bool isSiteAdmin)
        {
            IsContentAdmin = isContentAdmin;
            IsSiteAdmin = isSiteAdmin;
        }
    }

    internal DnnSiteAdminPermissions UserMayAdminThis(UserInfo user)
    {
        // Null-Check
        if (IsAnonymous(user)) return new DnnSiteAdminPermissions(false);

        // Super always AppAdmin
        if (user.IsSuperUser) return new DnnSiteAdminPermissions(true);

        var portal = PortalSettings.Current;

        // Skip the remaining tests if the portal isn't known
        if (portal == null) return new DnnSiteAdminPermissions(false);

        // Non-SuperUsers must be Admin AND in the group SxcAppAdmins
        if (!user.IsInRole(portal.AdministratorRoleName ?? "Administrators")) return new DnnSiteAdminPermissions(false);

        var hasSpecialGroup = PortalHasExplicitAdminGroups(portal.PortalId);
        if (hasSpecialGroup && IsExplicitAdmin(user)) return new DnnSiteAdminPermissions(true);

        // If the special group doesn't exist, then the admin-state (which is true - since he got here) is valid
        return new DnnSiteAdminPermissions(true, !hasSpecialGroup);
    }


    internal List<int> RoleList(UserInfo user, int? portalId = null) =>
        IsAnonymous(user) ? new List<int>() : user.Roles
            .Select(r => RoleController.Instance.GetRoleByName(portalId ?? user.PortalID, r))
            .Where(r => r != null)
            .Select(r => r.RoleID)
            .ToList();

    internal Guid UserGuid(UserInfo user) => Membership.GetUser(user.Username)?.ProviderUserKey as Guid? ?? Guid.Empty;

    internal string UserIdentityToken(UserInfo user) => IsAnonymous(user) ? Constants.Anonymous : DnnConstants.UserTokenPrefix + user.UserID;

    internal CmsUserRaw CmsUserBuilder(UserInfo user, int siteId)
    {
        var adminInfo = UserMayAdminThis(user);
        return new CmsUserRaw
        {
            Id = user.UserID,
            Guid = UserGuid(user),
            NameId = UserIdentityToken(user),
            Roles = RoleList(user, portalId: siteId),
            IsSystemAdmin = user.IsSuperUser,
            IsSiteAdmin = adminInfo.IsSiteAdmin,
            IsContentAdmin = adminInfo.IsContentAdmin,
            //IsDesigner = user.IsDesigner(),
            IsAnonymous = IsAnonymous(user),
            Created = user.CreatedOnDate,
            Modified = user.LastModifiedOnDate,
            //
            Username = user.Username,
            Email = user.Email,
            Name = user.DisplayName
        };
    }
}