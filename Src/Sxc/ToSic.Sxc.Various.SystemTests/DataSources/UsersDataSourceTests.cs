using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Cms.Users.Internal;

namespace ToSic.Sxc.DataSources;

// Note: most of these tests are useless ATM
// Reason is that previously the filtering for superusers etc. happened in the UserDataSource
// But it was then moved to a provider model.
// But it doesn't make sense to have a mock provider with these filters, and test for that,
// since that code would never be used in production.
// So for now, most of the tests are disabled
//
// In the future, we should find a way to system-test DNN DBs with real data, to make sure the filters work

public class UsersDataSourceTests(DataBuilder dataBuilder, DataSourcesTstBuilder DsSvc) : IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    public class Startup : StartupSxcWithDb
    {
        public override void ConfigureServices(IServiceCollection services) =>
            base.ConfigureServices(services.AddTransient<IUsersProvider, MockUsersProvider>());
    }

    [Fact]
    public void UsersDefault()
    {
        var usersDataSource = GenerateUsersDataSource();
        Equal(MockUsersProvider.GenerateTotal, usersDataSource.List.ToList().Count);
    }

    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-integer, -1", 17)]
    [InlineData("1", 0)]
    [InlineData("1,2,3,4,5,6,7,8,9,10", 7)]
    [InlineData("a,b,c,-2,-1,4,4,5,6,4", 3)]
    public void UsersWithIncludeUserIdsFilter(string includeUsersFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            UserIds = includeUsersFilter,
        });
        //usersDataSource.UserIds = includeUsersFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-guid, 00000000-0000-0000-0000-000000000000", 17)]
    [InlineData("00000000-0000-0000-0000-000000000005", 1)]
    [InlineData("00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000007", 2)]
    [InlineData("a,b,c,10,1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 4)]
    public void UsersWithIncludeUserGuidsFilter(string includeUsersFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            UserIds = includeUsersFilter,
        });
        //usersDataSource.UserIds = includeUsersFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-integer,-1", 17)]
    [InlineData("10", 16)]
    [InlineData("2,3,4,5", 15)]
    [InlineData("a,b,c,-2,-1,4,4,5,6,4", 14)]
    public void UsersWithExcludeUserIdsFilter(string excludeUsersFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            ExcludeUserIds = excludeUsersFilter,
        });
        //usersDataSource.ExcludeUserIds = excludeUsersFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-guid, 00000000-0000-0000-0000-000000000000", 17)]
    [InlineData("00000000-0000-0000-0000-000000000009", 16)]
    [InlineData("00000000-0000-0000-0000-000000000003,00000000-0000-0000-0000-000000000004", 16)]
    [InlineData("a,b,c,-2,-1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 14)]
    public void UsersWithExcludeUserGuidsFilter(string excludeUsersFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            ExcludeUserIds = excludeUsersFilter,
        });
        //usersDataSource.ExcludeUserIds = excludeUsersFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("not-a-guid, 00000000-0000-0000-0000-000000000000", "not-a-integer,-1", 17)]
    [InlineData("3,4,5", "00000000-0000-0000-0000-000000000001, 00000000-0000-0000-0000-000000000003, 00000000-0000-0000-0000-000000000004", 1)]
    [InlineData("00000000-0000-0000-0000-000000000007, 00000000-0000-0000-0000-000000000008,00000000-0000-0000-0000-000000000009", "7", 2)]
    public void UsersWithIncludeExcludeUsersFilter(string includeUsersFilter, string excludeUsersFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            UserIds = includeUsersFilter,
            ExcludeUsersFilter = excludeUsersFilter
        });
        //usersDataSource.UserIds = includeUsersFilter;
        //usersDataSource.ExcludeUserIds = excludeUsersFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-integer,-1,3", 7)]
    [InlineData("9", 11)]
    [InlineData("1,2", 7)]
    [InlineData("a,b,c,-2,-1,4,4,5,6,4", 3)]
    public void UsersWithIncludeRolesFilter(string includeRolesFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            RoleIds = includeRolesFilter
        });
        //usersDataSource.RoleIds = includeRolesFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-integer,-1,2", 10)]
    [InlineData("9", 6)]
    [InlineData("3,10", 0)]
    [InlineData("a,b,c,-2,-1,4,4,5,6,4", 14)]
    public void UsersWithExcludeRolesFilter(string excludeRolesFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            ExcludeRoleIds = excludeRolesFilter,
        });
        //usersDataSource.ExcludeRoleIds = excludeRolesFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("2,10", "3", 10)]
    public void UsersWithIncludeExcludeRolesFilter(string includeRolesFilter, string excludeRolesFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            RoleIds = includeRolesFilter,
            ExcludeRoleIds = excludeRolesFilter
        });
        //usersDataSource.RoleIds = includeRolesFilter;
        //usersDataSource.ExcludeRoleIds = excludeRolesFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("", 17)]
    [InlineData("not-a-bool", 17)]
    [InlineData("true", 20)]
    [InlineData("false", 17)]
    [InlineData("TRue", 20)]
    [InlineData("   false   ", 17)]
    [InlineData("1", 17)]
    [InlineData("0", 17)]
    [InlineData("-1", 17)]
    [InlineData("-100", 17)]
    [InlineData("yes", 17)]
    [InlineData("no", 17)]
    [InlineData("on", 17)]
    [InlineData("off", 17)]
    public void UsersWithSuperUserFilter(string superUserFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            IncludeSystemAdmins = superUserFilter
        });
        //usersDataSource.IncludeSystemAdmins = superUserFilter;
        //usersDataSource.Configuration.Values[nameof(usersDataSource.IncludeSystemAdmins)] = superUserFilter;
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    
    [Theory(Skip = "not in use ATM because of changes in the mechanims")]
    [InlineData("1,2,3,4,5,6,7,8,9,10", "00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", "2", "9", true, 6)]
    // TODO: this test doesn't seem to do much different than the first?
    [InlineData("1,2,3,4,5,6,7,8,9,10", "00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", "1,2", "9", false, 6)]
    public void UsersWithAllFilters(string includeUsersFilter, string excludeUsersFilter, string includeRolesFilter, string excludeRolesFilter, bool superUserFilter, int expected)
    {
        var usersDataSource = GenerateUsersDataSource(new
        {
            UserIds = includeUsersFilter,
            ExcludeUserIds = excludeUsersFilter,
            RoleIds = includeRolesFilter,
            ExcludeRoleIds = excludeRolesFilter,
            IncludeSystemAdmins = superUserFilter.ToString(),
        });
        //usersDataSource.UserIds = includeUsersFilter;
        //usersDataSource.ExcludeUserIds = excludeUsersFilter;
        //usersDataSource.RoleIds = includeRolesFilter;
        //usersDataSource.ExcludeRoleIds = excludeRolesFilter;
        //usersDataSource.IncludeSystemAdmins = superUserFilter.ToString();
        Equal(expected, usersDataSource.List.ToList().Count);
    }

    private Users GenerateUsersDataSource(object? options = default)
        => DsSvc.CreateDataSourceNew<Users>(new DataSourceOptionConverter()
            .Create(new DataSourceOptions
            {
                AppIdentityOrReader = new AppIdentity(0, 0),
                LookUp = new LookUpTestData(dataBuilder).AppSetAndRes(),
            }, options));
}