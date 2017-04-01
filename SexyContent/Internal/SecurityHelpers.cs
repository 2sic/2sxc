using DotNetNuke.Entities.Users;
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

    }
}