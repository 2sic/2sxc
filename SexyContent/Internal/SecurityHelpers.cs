using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;

namespace ToSic.SexyContent.Internal
{
    public class SecurityHelpers
    {
        /// <summary>
        /// Returns true if a DotNetNuke User Group "SexyContent Designers" exists and contains at minumum one user
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static bool SexyContentDesignersGroupConfigured(int portalId)
        {
            var roleControl = new RoleController();
            var role = roleControl.GetRoleByName(portalId, Settings.SexyContentGroupName);
            return role != null;
        }

        /// <summary>
        /// Returns true if a user is in the SexyContent Designers group
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsInSexyContentDesignersGroup(UserInfo user)
        {
            return user.IsInRole(Settings.SexyContentGroupName);
        }

        // todo: probably same functionality as Environment.Permissions.UserMayEditContent ??? 
        /// <summary>
        /// Returns true if the user is able to edit this module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static bool HasEditPermission(ModuleInfo module)
        {
            // Make sure that HasEditPermission still works while search indexing
            if (PortalSettings.Current == null)
                return false;
            return ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "CONTENT", module);
        }
    }
}