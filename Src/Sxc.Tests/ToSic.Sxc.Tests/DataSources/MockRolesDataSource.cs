using System;
using System.Collections.Generic;
using ToSic.Eav.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources
{
    /// <summary>
    /// Mock a list of roles
    /// </summary>
    public class MockRolesDataSource : Sxc.DataSources.CmsBases.RolesDataSourceBase
    {
        protected override IEnumerable<RoleDataSourceInfo> GetRolesInternal()
        {
            var wrapLog = Log.Fn<List<RoleDataSourceInfo>>();
            const int siteId = 0;
            Log.A($"Mock Portal Id {siteId}");

            var result = new List<RoleDataSourceInfo>();
            for (var i = 0; i < 10; i++)
            {
                result.Add(new RoleDataSourceInfo
                {
                    Id = i,
                    Name = $"[role_name_{i}]",
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                });
            }

            return wrapLog.Return(result, "found");
        }
    }
}