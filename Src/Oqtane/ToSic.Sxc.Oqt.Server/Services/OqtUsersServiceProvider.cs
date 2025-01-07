using Oqtane.Repository;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Oqt.Server.Services;

internal class OqtUsersServiceProvider(
    LazySvc<IUserRepository> userRepository,
    LazySvc<IUserRoleRepository> userRoleRepository,
    LazySvc<OqtSecurity> oqtSecurity)
    : UserSourceProvider("Oqt.UsersSvc", connect: [userRepository, userRoleRepository, oqtSecurity])
{
    public override string PlatformIdentityTokenPrefix => OqtConstants.UserTokenPrefix;

    /// <summary>
    /// Get user information from platform
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="siteId"></param>
    /// <returns></returns>
    /// <remarks>
    /// based on Oqtane.Managers.UserManager.GetUser(int userid, int siteid)
    /// </remarks>
    internal override ICmsUser PlatformUserInformationDto(int userId, int siteId)
    {
        var user = userRepository.Value.GetUser(userId, false);
        if (user == null) return null;
        user.SiteId = siteId;
        user.Roles = GetUserRoles(userId, siteId);
        return oqtSecurity.Value.CmsUserBuilder(user);
    }

    /// <summary>
    /// Get roles for user in site
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="siteId"></param>
    /// <returns></returns>
    /// <remarks>
    /// based on Oqtane.Managers.UserManager.GetUserRoles
    /// </remarks>
    private string GetUserRoles(int userId, int siteId)
    {
        const string adminRoleName = Oqtane.Shared.RoleNames.Admin; // "Administrators";
        var userRoles = "";
        var list = userRoleRepository.Value.GetUserRoles(userId, siteId).ToList();
        foreach (var userRole in list)
        {
            userRoles = userRoles + userRole.Role.Name + ";";
            if (userRole.Role.Name == "Host Users" &&
                list.FirstOrDefault(item => item.Role.Name == adminRoleName) == null)
                userRoles += $"{adminRoleName};";
        }
        if (userRoles != "")
            userRoles = ";" + userRoles;
        return userRoles;
    }
}