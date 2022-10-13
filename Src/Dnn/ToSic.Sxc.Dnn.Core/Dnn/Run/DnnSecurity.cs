using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace ToSic.Sxc.Dnn.Run
{
    public static class DnnSecurity
    {
        /// <summary>
        /// Returns true if a DotNetNuke User Group "2sxc Designers" exists
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static bool PortalHasDesignersGroup(int portalId)
            => PortalHasGroup(portalId, Settings.DnnGroupSxcDesigners);


        public static bool PortalHasGroup(int portalId, string groupName)
            => new RoleController().GetRoleByName(portalId, groupName) != null;

        public static bool IsAnonymous(this UserInfo user) =>
            user == null || user.UserID == -1;

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

        internal static DnnSiteAdminPermissions UserMayAdminThis(this UserInfo user)
        {
            // Null-Check
            if (user.IsAnonymous()) return new DnnSiteAdminPermissions(false);

            // Super always AppAdmin
            if (user.IsSuperUser) return new DnnSiteAdminPermissions(true);

            var portal = PortalSettings.Current;

            // Skip the remaining tests if the portal isn't known
            if (portal == null) return new DnnSiteAdminPermissions(false);

            // Non-SuperUsers must be Admin AND in the group SxcAppAdmins
            if (!user.IsInRole(portal.AdministratorRoleName ?? "Administrators")) return new DnnSiteAdminPermissions(false);

            var hasSpecialGroup = PortalHasGroup(portal.PortalId, Settings.DnnGroupSxcDesigners);
            if (hasSpecialGroup && IsDesigner(user)) return new DnnSiteAdminPermissions(true);

            hasSpecialGroup = hasSpecialGroup || PortalHasGroup(portal.PortalId, Settings.DnnGroupSxcAdmins);
            if (hasSpecialGroup && user.IsInRole(Settings.DnnGroupSxcAdmins)) return new DnnSiteAdminPermissions(true);

            // If the special group doesn't exist, then the admin-state (which is true - since he got here) is valid
            return new DnnSiteAdminPermissions(true, !hasSpecialGroup);
        }

        public static bool IsDesigner(this UserInfo user) =>
            !user.IsAnonymous() && user.IsInRole(Settings.DnnGroupSxcDesigners);

        public static List<int> RoleList(this UserInfo user, int? portalId = null) =>
            user.IsAnonymous() ? new List<int>() : user.Roles
                .Select(r => RoleController.Instance.GetRoleByName(portalId ?? user.PortalID, r))
                .Where(r => r != null)
                .Select(r => r.RoleID)
                .ToList();

        public static Guid? UserGuid(this UserInfo user) => 
            Membership.GetUser(user.Username)?.ProviderUserKey as Guid?;

        public static string UserIdentityToken(this UserInfo user) => 
            user.IsAnonymous() ? Constants.Anonymous : DnnConstants.UserTokenPrefix + user.UserID;
    }
}