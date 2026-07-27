using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources;

/// <summary>
/// Mock a list of roles
/// </summary>
public class MockUserRolesProvider() : ServiceBase("DS.MockRoles"), IUserRolesProvider
{
    public IEnumerable<UserRoleModelRaw> GetRoles()
    {
        var l = Log.Fn<IEnumerable<UserRoleModelRaw>>();
        const int siteId = 0;
        l.A($"Mock Portal Id {siteId}");

        var roles = new List<UserRoleModelRaw>();
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