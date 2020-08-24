using System;
using System.Security.Authentication;
using DotNetNuke.Entities.Portals;

namespace ToSic.Sxc.Security
{
    public static class RunIf
    {
        internal static T Admin<T>(PortalSettings portal, Func<T> task)
        {
            ThrowIfNotAdmin(portal);
            return task.Invoke();
        }

        internal static void ThrowIfNotAdmin(PortalSettings portal)
        {
            if (!portal.UserInfo.IsInRole(portal.AdministratorRoleName))
                throw new AuthenticationException("Needs admin permissions to do this");
        }
    }
}
