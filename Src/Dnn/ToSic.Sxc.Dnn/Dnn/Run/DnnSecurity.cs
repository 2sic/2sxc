using DotNetNuke.Security.Roles;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnSecurity
    {
        /// <summary>
        /// Returns true if a DotNetNuke User Group "SexyContent Designers" exists and contains at minimum one user
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        public static bool SexyContentDesignersGroupConfigured(int portalId)
            => new RoleController()
                   .GetRoleByName(portalId, Settings.SexyContentGroupName)
               != null;

        ///// <summary>
        ///// Returns true if a user is in the SexyContent Designers group
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public static bool IsInSexyContentDesignersGroup(IUser user) 
        //    => user.IsInRole(Settings.SexyContentGroupName);
    }
}