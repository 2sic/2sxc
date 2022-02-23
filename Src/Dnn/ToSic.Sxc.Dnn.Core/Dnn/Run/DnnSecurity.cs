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


        public static bool UserMayAdminThis(this UserInfo user)
        {
            // Null-Check
            if (user == null) return false;

            // Super always AppAdmin
            if (user.IsSuperUser) return true;

            var portal = PortalSettings.Current;

            // Non-SuperUsers must be Admin AND in the group SxcAppAdmins
            if (!user.IsInRole(portal.AdministratorRoleName ?? "Administrators")) return false;

            // Skip the remaining tests if the portal isn't known
            if (portal == null) return false;

            var hasSpecialGroup = PortalHasGroup(portal.PortalId, Settings.DnnGroupSxcDesigners);
            if (hasSpecialGroup && user.IsInRole(Settings.DnnGroupSxcDesigners)) return true;

            hasSpecialGroup = hasSpecialGroup || PortalHasGroup(portal.PortalId, Settings.DnnGroupSxcAdmins);
            if (hasSpecialGroup && user.IsInRole(Settings.DnnGroupSxcAdmins)) return true;

            // If the special group doesn't exist, then the admin-state (which is true - since he got here) is valid
            return !hasSpecialGroup;
        }
    }
}