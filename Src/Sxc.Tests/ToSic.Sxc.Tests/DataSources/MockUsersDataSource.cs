using System;
using System.Collections.Generic;
using ToSic.Eav.Logging;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Mock list of users
    /// </summary>
    public class MockUsersDataSource : CmsBases.UsersDataSourceBase
    {
        protected override IEnumerable<UserDataSourceInfo> GetUsersInternal()
        {
            var wrapLog = Log.Fn<List<UserDataSourceInfo>>();
            var siteId = 0;
            Log.A($"Portal Id {siteId}");
            var result = new List<UserDataSourceInfo>();

            // super users and admins
            for (var i = 1; i <= 3; i++)
            {
                result.Add(new UserDataSourceInfo
                {
                    Id = i,
                    Guid = new Guid($"00000000-0000-0000-0000-{i:d12}"),
                    IdentityToken = $"mock:{i}",
                    Roles = new List<int>() { 1 },
                    IsSuperUser = true,
                    IsSiteAdmin = true,
                    IsDesigner = false,
                    IsAnonymous = false,
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                    //
                    Username = $"superuser{i}",
                    Email = $"superuser{i}@email.com",
                    Name = $"DNSuperuser{i}"
                });
            }

            // with 3 roles [ 2, 3, 4-10]
            for (var i = 4; i <= 10; i++)
            {
                result.Add(new UserDataSourceInfo
                {
                    Id = i,
                    Guid = new Guid($"00000000-0000-0000-0000-{i:d12}"),
                    IdentityToken = $"mock:{i}",
                    Roles = new List<int> { 2, 3, i },
                    IsSuperUser = false,
                    IsSiteAdmin = false,
                    IsDesigner = false,
                    IsAnonymous = false,
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                    //
                    Username = $"username{i}",
                    Email = $"username{i}@email.com",
                    Name = $"Displayname{i}"
                });
            }

            // with 2 roles [ 9, 10]
            for (var i = 11; i <= 20; i++)
            {
                result.Add(new UserDataSourceInfo
                {
                    Id = i,
                    Guid = new Guid($"00000000-0000-0000-0000-{i:d12}"),
                    IdentityToken = $"mock:{i}",
                    Roles = new List<int> { 9, 10 },
                    IsSuperUser = false,
                    IsSiteAdmin = false,
                    IsDesigner = false,
                    IsAnonymous = false,
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                    //
                    Username = $"username{i}",
                    Email = $"username{i}@email.com",
                    Name = $"Displayname{i}"
                });
            }

            return wrapLog.Return(result, "found");
        }
    }
}