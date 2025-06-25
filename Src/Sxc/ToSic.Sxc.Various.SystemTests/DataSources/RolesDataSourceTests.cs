using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources;

public class RolesDataSourceTests(DataBuilder dataBuilder, DataSourcesTstBuilder dsSvc) : IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    public class Startup: StartupSxcWithDb
    {
        public override void ConfigureServices(IServiceCollection services) => 
            base.ConfigureServices(services.AddTransient<IUserRolesProvider, MockUserRolesProvider>());
    }

    [Fact]
    public void RolesDefault()
    {
        var rolesDataSource = GenerateRolesDataSourceDataSource();
        Equal(10, rolesDataSource.List.ToList().Count);
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("not-a-integer,-1", 0)]
    [InlineData("1", 1)]
    [InlineData("2,3", 2)]
    [InlineData("a,b,c,-2,-1,4,4,5,6,4", 3)]
    public void RolesWithIncludeFilter(string includeRolesFilter, int expected)
    {
        var rolesDataSource = GenerateRolesDataSourceDataSource(new
        {
            RoleIds = includeRolesFilter
        });
        //rolesDataSource.RoleIds = includeRolesFilter;
        Equal(expected, rolesDataSource.List.ToList().Count);
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("not-a-integer,-1", 10)]
    [InlineData("1", 9)]
    [InlineData("2,3", 8)]
    [InlineData("a,b,c,-2,-1,4,4,5,6,4", 7)]
    public void RolesWithExcludeFilter(string excludeRolesFilter, int expected)
    {
        var rolesDataSource = GenerateRolesDataSourceDataSource(new
        {
            ExcludeRoleIds = excludeRolesFilter
        });
        //rolesDataSource.ExcludeRoleIds = excludeRolesFilter;
        Equal(expected, rolesDataSource.List.ToList().Count);
    }

    [Theory]
    [InlineData("", "", 10)]
    [InlineData("not-a-integer,-1", "not-a-integer,-1", 0)]
    [InlineData("3,4,5", "1,2,3", 2)]
    public void RolesWithIncludeExcludeFilter(string includeRolesFilter, string excludeRolesFilter, int expected)
    {
        var rolesDataSource = GenerateRolesDataSourceDataSource(new
        {
            RoleIds = includeRolesFilter,
            ExcludeRoleIds = excludeRolesFilter
        });
        //rolesDataSource.RoleIds = includeRolesFilter;
        //rolesDataSource.ExcludeRoleIds = excludeRolesFilter;
        Equal(expected, rolesDataSource.List.ToList().Count);
    }

    private UserRoles GenerateRolesDataSourceDataSource(object? options = default) 
        => dsSvc.CreateDataSourceNew<UserRoles>(new DataSourceOptionConverter()
            .Create(new DataSourceOptions
            {
                AppIdentityOrReader = new AppIdentity(0, 0),
                LookUp = new LookUpTestData(dataBuilder).AppSetAndRes()
            }, options));
}