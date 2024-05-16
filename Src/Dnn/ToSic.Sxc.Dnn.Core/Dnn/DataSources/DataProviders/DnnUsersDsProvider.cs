using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Dnn.Run;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal class DnnUsersDsProvider : UsersDataSourceProvider
{
    private readonly LazySvc<DnnSecurity> _dnnSecurity;

    public DnnUsersDsProvider(LazySvc<DnnSecurity> dnnSecurity) : base("Dnn.Users")
    {
        ConnectLogs([
            _dnnSecurity = dnnSecurity
        ]);
    }

    public override IEnumerable<CmsUserRaw> GetUsersInternal()
    {
        var l = Log.Fn<IEnumerable<CmsUserRaw>>();
        var siteId = PortalSettings.Current?.PortalId ?? -1;
        l.A($"Portal Id {siteId}");
        try
        {
            // take all portal users (this should include superusers, but superusers are missing)
            var dnnAllUsers =
                UserController.GetUsers(portalId: siteId, includeDeleted: false, superUsersOnly: false);

            // append all superusers
            dnnAllUsers.AddRange(UserController.GetUsers(portalId: -1, includeDeleted: false,
                superUsersOnly: true));

            var dnnUsers = dnnAllUsers.Cast<UserInfo>().ToList();
            if (!dnnUsers.Any()) return l.Return(new List<CmsUserRaw>(), "null/empty");

            var result = dnnUsers
                //.Where(user => !user.IsDeleted)
                .Select(u => _dnnSecurity.Value.CmsUserBuilder(u, siteId)).ToList();
            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<CmsUserRaw>(), "error");
        }
    }
}