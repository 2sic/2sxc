using System;
using System.Collections.Generic;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context.Internal.Raw;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Mock list of users
    /// </summary>
    public class MockUsersDataSource : UsersDataSourceProvider
    {
        public MockUsersDataSource() : base("DS.MockUsers") { }

        public override IEnumerable<CmsUserRaw> GetUsersInternal() => Log.Func(l =>
        {
            var siteId = 0;
            l.A($"Portal Id {siteId}");
            var users = new List<CmsUserRaw>();

            l.A($"mock 3 super users and admins with one role [1-3]");
            for (var i = 1; i <= 3; i++)
            {
                users.Add(new()
                {
                    Id = i,
                    Guid = new($"00000000-0000-0000-0000-{i:d12}"),
                    NameId = $"mock:{i}",
                    Roles = new() {i},
                    IsSystemAdmin = true,
                    IsSiteAdmin = true,
                    //IsDesigner = false,
                    IsAnonymous = false,
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                    //
                    Username = $"superuser{i}",
                    Email = $"superuser{i}@email.com",
                    Name = $"DNSuperuser{i}"
                });
            }


            l.A($"mock 7 normal users with 3 roles [ 2, 3, 4-10]");
            for (var i = 4; i <= 10; i++)
            {
                users.Add(new()
                {
                    Id = i,
                    Guid = new($"00000000-0000-0000-0000-{i:d12}"),
                    NameId = $"mock:{i}",
                    Roles = new() {2, 3, i},
                    IsSystemAdmin = false,
                    IsSiteAdmin = false,
                    //IsDesigner = false,
                    IsAnonymous = false,
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                    //
                    Username = $"username{i}",
                    Email = $"username{i}@email.com",
                    Name = $"Displayname{i}"
                });
            }

            l.A($"mock 10 normal users with 2 roles [9, 10]");
            for (var i = 11; i <= 20; i++)
            {
                users.Add(new()
                {
                    Id = i,
                    Guid = new($"00000000-0000-0000-0000-{i:d12}"),
                    NameId = $"mock:{i}",
                    Roles = new() {9, 10},
                    IsSystemAdmin = false,
                    IsSiteAdmin = false,
                    //IsDesigner = false,
                    IsAnonymous = false,
                    Created = DateTime.Today,
                    Modified = DateTime.Now,
                    //
                    Username = $"username{i}",
                    Email = $"username{i}@email.com",
                    Name = $"Displayname{i}"
                });
            }

            return (users, $"mock: {users.Count}");
        });
    }
}