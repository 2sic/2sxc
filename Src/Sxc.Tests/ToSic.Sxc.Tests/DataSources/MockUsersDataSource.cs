using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Html5;


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

            for (var i = 1; i <= 20; i++)
            {
                result.Add(new UserDataSourceInfo
                {
                    Id = i,
                    Guid = new Guid($"00000000-0000-0000-0000-{i:d12}"),
                    IdentityToken = $"mock:{i}",
                    Roles = new List<int>(),
                    IsSuperUser = false,
                    IsAdmin = false,
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