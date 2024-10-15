using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Lib.DI;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Oqt.Server.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal class OqtUsersDsProvider : UsersDataSourceProvider
{
    private readonly SiteState _siteState;
    private readonly LazySvc<OqtSecurity> _oqtSecurity;
    private readonly LazySvc<IUserRoleRepository> _userRoles;

    public OqtUsersDsProvider(SiteState siteState, LazySvc<OqtSecurity> oqtSecurity, LazySvc<IUserRoleRepository> userRoles) : base("Oqt.Users")
    {
        ConnectLogs([
            _siteState = siteState,
            _oqtSecurity = oqtSecurity,
            _userRoles = userRoles
        ]);
    }

    public override IEnumerable<CmsUserRaw> GetUsersInternal()
    {
        var l = Log.Fn<IEnumerable<CmsUserRaw>>();
        var siteId = _siteState.Alias.SiteId;
        l.A($"Portal Id {siteId}");
        try
        {
            var userRoles = _userRoles.Value.GetUserRoles(siteId).ToList();
            var users = userRoles.Select(ur => ur.User).Distinct().ToList();
            if (!users.Any()) return l.Return(new List<CmsUserRaw>(), "null/empty");

            var result = users
                .Where(u => !u.IsDeleted)
                .Select(u => _oqtSecurity.Value.CmsUserBuilder(u)).ToList();
            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<CmsUserRaw>(), "error");
        }
    }
}