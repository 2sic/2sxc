using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources;

/// <summary>
/// Mock a list of roles
/// </summary>
public class MockUserRolesProvider() : ServiceBase("DS.MockRoles"), IUserRolesProvider
{
    public IEnumerable<UserRoleModel> GetRoles()
    {
        var l = Log.Fn<IEnumerable<UserRoleModel>>();
        const int siteId = 0;
        l.A($"Mock Portal Id {siteId}");

        var roles = new List<UserRoleModel>();
        for (var i = 1; i <= 10; i++)
        {
            roles.Add(new()
            {
                Id = i,
                Name = $"[role_name_{i}]",
                Created = DateTime.Today,
                Modified = DateTime.Now,
            });
        }

        return l.Return(roles, $"mock: {roles.Count}");
    }
}