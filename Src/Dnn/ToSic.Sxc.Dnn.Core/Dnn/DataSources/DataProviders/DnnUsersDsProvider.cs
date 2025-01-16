using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Models.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

internal class DnnUsersDsProvider(LazySvc<DnnSecurity> dnnSecurity)
    : UsersDataSourceProvider("Dnn.Users", connect: [dnnSecurity])
{
    public override IEnumerable<UserModel> GetUsersInternal()
    {
        var l = Log.Fn<IEnumerable<UserModel>>();
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
            if (!dnnUsers.Any())
                return l.Return(new List<UserModel>(), "null/empty");

            var result = dnnUsers
                //.Where(user => !user.IsDeleted)
                .Select(u => dnnSecurity.Value.CmsUserBuilder(u, siteId))
                .ToList();

            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(new List<UserModel>(), "error");
        }
    }
}