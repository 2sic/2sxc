using System;
using System.Collections.Generic;
using ToSic.Lib.Logging;
using ToSic.Sxc.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources
{
    /// <summary>
    /// Mock a list of roles
    /// </summary>
    public class MockRolesDataSource : RolesDataSourceProvider
    {
        public override IEnumerable<CmsRoleInfo> GetRolesInternal() => Log.Func(l =>
        {
            const int siteId = 0;
            Log.A($"Mock Portal Id {siteId}");

            var roles = new List<CmsRoleInfo>();
            for (var i = 1; i <= 10; i++)
            {
                roles.Add(new CmsRoleInfo
                {
                    Id = i,
                    Name = $"[role_name_{i}]",
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                });
            }

            return (roles, $"mock: {roles.Count}");
        });

        public MockRolesDataSource() : base("DS.MockRoles")
        {
        }
    }
}