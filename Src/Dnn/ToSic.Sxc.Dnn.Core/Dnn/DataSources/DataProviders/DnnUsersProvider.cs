using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using System.Collections;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using static DotNetNuke.Common.Utilities.Null;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal class DnnUsersProvider(LazySvc<DnnSecurity> dnnSecurity)
    : ServiceBase("Dnn.Users", connect: [dnnSecurity]), IUsersProvider
{
    #region Configuration
    private UsersGetSpecs _specs;
    private UsersGetSpecsParsed SpecsParsed => field ??= new(_specs);
    #endregion

    public string PlatformIdentityTokenPrefix => DnnConstants.UserTokenPrefix;

    public IUserModel GetUser(int userId, int siteId)
    {
        var user = UserController.Instance.GetUserById(siteId, userId);
        return user == null
            ? null
            : dnnSecurity.Value.CmsUserBuilder(user, siteId);
    }

    public IEnumerable<UserModel> GetUsers(UsersGetSpecs specs)
    {
        var l = Log.Fn<IEnumerable<UserModel>>($"specs:{specs}");
        _specs = specs;
        try
        {
            return l.Return(GetUsersInternal(), "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<UserModel>(), "error");
        }
    }

    private IEnumerable<UserModel> GetUsersInternal()
    {
        var l = Log.Fn<List<UserModel>>();
        var siteId = PortalSettings.Current?.PortalId ?? NullInteger;
        l.A($"Portal Id {siteId}");
        try
        {
            var dnnUsers = new List<UserInfo>();

            // Include users
            if (_specs.UserIds.IsEmpty() && _specs.RoleIds.IsEmpty())
            {
                var dnnAllUsers = new ArrayList();

                if (!_specs.IncludeSystemAdmins.EqualsInsensitive(UserConstants.IncludeRequired))
                    // take all portal users (this should include superusers, but superusers are missing)
                    dnnAllUsers.AddRange(UserController.GetUsers(portalId: siteId, includeDeleted: false, superUsersOnly: false));

                if (!_specs.IncludeSystemAdmins.EqualsInsensitive(UserConstants.IncludeForbidden))
                    // append all superusers
                    dnnAllUsers.AddRange(UserController.GetUsers(portalId: -1, includeDeleted: false, superUsersOnly: true));

                dnnUsers.AddRange(dnnAllUsers.Cast<UserInfo>().ToList());
            }
            else
            {
                // UserIds
                dnnUsers.AddRange(SpecsParsed.UserIdFilter.Except(SpecsParsed.ExcludeUserIdsFilter)
                    .Select(userId => UserController.GetUserById(siteId, userId)));

                dnnUsers.AddRange(SpecsParsed.UserGuidFilter.Except(SpecsParsed.ExcludeUserGuidsFilter)
                    .Select(membershipUserKey => GetUserByMembershipUserKey(siteId, membershipUserKey)));

                // RoleIds
                dnnUsers.AddRange(SpecsParsed.RolesFilter.Except(SpecsParsed.ExcludeRolesFilter)
                    .SelectMany(roleId => GetUsersByRoleId(siteId, roleId)));
            }

            // Exclude users
            dnnUsers = dnnUsers.Distinct().Where(user => !ExcludeUser(user)).ToList();

            if (!dnnUsers.Any())
                return l.Return([], "null/empty");

            var users = dnnUsers
                //.Where(user => !user.IsDeleted)
                .Select(u => dnnSecurity.Value.CmsUserBuilder(u, siteId))
                .ToList();

            return l.Return(users, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return([], "error");
        }
    }

    private bool ExcludeUser(UserInfo user)
    {
        if (user == null) return true;
        if (SpecsParsed.ExcludeUserIdsFilter.Contains(user.UserID)) return true;
        if (SpecsParsed.ExcludeUserGuidsFilter.Contains(dnnSecurity.Value.UserGuid(user))) return true;
        if (SpecsParsed.ExcludeRolesFilter.Any(roleId => user.IsInRole(RoleController.Instance.GetRoleById(user.PortalID, roleId).RoleName))) return true;
        if (_specs.IncludeSystemAdmins.EqualsInsensitive(UserConstants.IncludeForbidden) && user.IsSuperUser) return true;
        if (_specs.IncludeSystemAdmins.EqualsInsensitive(UserConstants.IncludeRequired) && !user.IsSuperUser) return true;
        return false;
    }

    private static IList<UserInfo> GetUsersByRoleId(int siteId, int roleId)
        => UserController.Instance
            .GetUsersAdvancedSearch(
                portalId: siteId,
                userId: NullInteger,
                filterUserId: NullInteger,
                filterRoleId: roleId,
                relationTypeId: NullInteger,
                isAdmin: false,
                pageIndex: 0,
                pageSize: NullInteger,
                sortColumn: null,
                sortAscending: true,
                propertyNames: null,
                propertyValues: null
            )
            .ToList();

    private static UserInfo GetUserByMembershipUserKey(int portalId, Guid membershipUserKey)
    {
        var masterPortalId = PortalController.GetEffectivePortalId(portalId);
        var user = MembershipProvider.Instance().GetUserByProviderUserKey(masterPortalId, membershipUserKey.ToString());
        if (user != null)
            user.PortalID = portalId;
        return user;
    }
}