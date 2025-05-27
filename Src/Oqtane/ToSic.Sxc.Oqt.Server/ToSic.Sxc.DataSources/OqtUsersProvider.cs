using Oqtane.Managers;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Internal;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Utils;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal class OqtUsersProvider(
    SiteState siteState,
    LazySvc<OqtSecurity> oqtSecurity,
    LazySvc<IUserRoleRepository> userRolesRepository,
    LazySvc<UserManager> userManager)
    : ServiceBase("Oqt.Users", connect: [siteState, oqtSecurity, userRolesRepository, userManager]), IUsersProvider
{
    #region Configuration
    private UsersGetSpecs _specs;

    private UsersGetSpecsParsed SpecsParsed => field ??= new(_specs);

    #endregion

    public string PlatformIdentityTokenPrefix => OqtConstants.UserTokenPrefix;

    public IUserModel GetUser(int userId, int siteId)
    {
        var user = userManager.Value.GetUser(userId, SiteId);
        return user == null 
            ? null 
            : oqtSecurity.Value.CmsUserBuilder(user);
    }

    public IEnumerable<UserModel> GetUsers(UsersGetSpecs specs)
    {
        var l = Log.Fn<List<UserModel>>();
        _specs = specs;

        l.A($"Portal Id {SiteId}");

        try
        {
            var oqtUserRoles = new List<UserRole>();

            // Include users
            if (_specs.UserIds.IsEmpty() && _specs.RoleIds.IsEmpty())
            {
                oqtUserRoles.AddRange(OqtAllUserRoles);
            }
            else
            {
                // UserIds
                oqtUserRoles.AddRange(SpecsParsed.UserIdFilter.Except(SpecsParsed.ExcludeUserIdsFilter)
                    .SelectMany(userId => userRolesRepository.Value.GetUserRoles(userId, SiteId)));

                oqtUserRoles.AddRange(SpecsParsed.UserGuidFilter.Except(SpecsParsed.ExcludeUserGuidsFilter)
                    .SelectMany(membershipUserKey => OqtAllUserRoles.Where(ur => oqtSecurity.Value.UserGuid(ur.User.Username) == membershipUserKey)));

                // RoleIds
                oqtUserRoles.AddRange(SpecsParsed.RolesFilter.Except(SpecsParsed.ExcludeRolesFilter)
                    .SelectMany(roleId => OqtAllUserRoles.Where(ur => ur.RoleId == roleId)));
            }

            // Excluded users
            var excludedUsers = oqtUserRoles
                .Where(ExcludeUser)
                .Select(ur => ur.UserId)
                .Distinct();

            // results
            var oqtUsers = oqtUserRoles
                .Select(ur => ur.UserId)
                .Where(userId => !excludedUsers.Contains(userId))
                .Distinct()
                .Select(userId => userManager.Value.GetUser(userId, SiteId))
                .ToList();

            if (!oqtUsers.Any())
                return l.Return([], "null/empty");

            var users = oqtUsers
                //.Where(user => !user.IsDeleted)
                .Select(u => oqtSecurity.Value.CmsUserBuilder(u))
                .ToList();

            return l.Return(users, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return([], "error");
        }
    }

    private int SiteId => siteState.Alias.SiteId;

    private IEnumerable<UserRole> OqtAllUserRoles => _oqtAllUserRoles.Get(() => userRolesRepository.Value.GetUserRoles(SiteId));
    private readonly GetOnce<IEnumerable<UserRole>> _oqtAllUserRoles = new();

    private bool ExcludeUser(UserRole userRole)
    {
        if (userRole == null) return true;
        if (SpecsParsed.ExcludeUserIdsFilter.Contains(userRole.UserId)) return true;
        if (SpecsParsed.ExcludeUserGuidsFilter.Contains(oqtSecurity.Value.UserGuid(userRole.User.Username))) return true;
        if (SpecsParsed.ExcludeRolesFilter.Contains(userRole.RoleId)) return true;
        if (_specs.IncludeSystemAdmins.EqualsInsensitive(UserConstants.IncludeForbidden) && IsSystemAdmin(userRole)) return true;
        if (_specs.IncludeSystemAdmins.EqualsInsensitive(UserConstants.IncludeRequired) && !IsSystemAdmin(userRole)) return true;
        return false;
    }

    private bool IsSystemAdmin(UserRole userRole) => userRole.RoleId == 2; // Host Users
}