using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Lib.DI;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Models.Internal;
using ToSic.Sxc.Oqt.Server.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal class OqtUsersDsProvider(
    SiteState siteState,
    LazySvc<OqtSecurity> oqtSecurity,
    LazySvc<IUserRoleRepository> roles)
    : UsersDataSourceProvider("Oqt.Users", connect: [siteState, oqtSecurity, roles])
{
    public override IEnumerable<UserRaw> GetUsersInternal()
    {
        var l = Log.Fn<IEnumerable<UserRaw>>();
        var siteId = siteState.Alias.SiteId;
        l.A($"Portal Id {siteId}");
        try
        {
            var userRoles = roles.Value.GetUserRoles(siteId).ToList();
            var users = userRoles.Select(ur => ur.User).Distinct().ToList();
            if (!users.Any()) return l.Return(new List<UserRaw>(), "null/empty");

            var result = users
                .Where(u => !u.IsDeleted)
                .Select(u => oqtSecurity.Value.CmsUserBuilder(u)).ToList();
            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<UserRaw>(), "error");
        }
    }
}