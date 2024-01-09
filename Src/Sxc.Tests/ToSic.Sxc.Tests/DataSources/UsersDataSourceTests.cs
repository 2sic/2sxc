using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToSic.Eav.Core.Tests.LookUp;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources
{
    [TestClass]
    public class UsersDataSourceTests : TestBaseSxcDb
    {
        // Start the test with a platform-info that has a patron
        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddTransient<UsersDataSourceProvider, MockUsersDataSource>();
        }


        [TestMethod()]
        public void UsersDefault()
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            Assert.AreEqual(17, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-integer, -1", 17)]
        [DataRow("1", 0)]
        [DataRow("1,2,3,4,5,6,7,8,9,10", 7)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 3)]
        public void UsersWithIncludeUserIdsFilter(string includeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                UserIds = includeUsersFilter,
            });
            //usersDataSource.UserIds = includeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", 17)]
        [DataRow("00000000-0000-0000-0000-000000000005", 1)]
        [DataRow("00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000007", 2)]
        [DataRow("a,b,c,10,1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 4)]
        public void UsersWithIncludeUserGuidsFilter(string includeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                UserIds = includeUsersFilter,
            });
            //usersDataSource.UserIds = includeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-integer,-1", 17)]
        [DataRow("10", 16)]
        [DataRow("2,3,4,5", 15)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 14)]
        public void UsersWithExcludeUserIdsFilter(string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                ExcludeUserIds = excludeUsersFilter,
            });
            //usersDataSource.ExcludeUserIds = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", 17)]
        [DataRow("00000000-0000-0000-0000-000000000009", 16)]
        [DataRow("00000000-0000-0000-0000-000000000003,00000000-0000-0000-0000-000000000004", 16)]
        [DataRow("a,b,c,-2,-1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 14)]
        public void UsersWithExcludeUserGuidsFilter(string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                ExcludeUserIds = excludeUsersFilter,
            });
            //usersDataSource.ExcludeUserIds = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", "not-a-integer,-1", 17)]
        [DataRow("3,4,5", "00000000-0000-0000-0000-000000000001, 00000000-0000-0000-0000-000000000003, 00000000-0000-0000-0000-000000000004", 1)]
        [DataRow("00000000-0000-0000-0000-000000000007, 00000000-0000-0000-0000-000000000008,00000000-0000-0000-0000-000000000009", "7", 2)]
        public void UsersWithIncludeExcludeUsersFilter(string includeUsersFilter, string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                UserIds = includeUsersFilter,
                ExcludeUsersFilter = excludeUsersFilter
            });
            //usersDataSource.UserIds = includeUsersFilter;
            //usersDataSource.ExcludeUserIds = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-integer,-1,3", 7)]
        [DataRow("9", 11)]
        [DataRow("1,2", 7)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 3)]
        public void UsersWithIncludeRolesFilter(string includeRolesFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                RoleIds = includeRolesFilter
            });
            //usersDataSource.RoleIds = includeRolesFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-integer,-1,2", 10)]
        [DataRow("9", 6)]
        [DataRow("3,10", 0)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 14)]
        public void UsersWithExcludeRolesFilter(string excludeRolesFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                ExcludeRoleIds = excludeRolesFilter,
            });
            //usersDataSource.ExcludeRoleIds = excludeRolesFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("2,10", "3", 10)]
        public void UsersWithIncludeExcludeRolesFilter(string includeRolesFilter, string excludeRolesFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                RoleIds = includeRolesFilter,
                ExcludeRoleIds = excludeRolesFilter
            });
            //usersDataSource.RoleIds = includeRolesFilter;
            //usersDataSource.ExcludeRoleIds = excludeRolesFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 17)]
        [DataRow("not-a-bool", 17)]
        [DataRow("true", 20)]
        [DataRow("false", 17)]
        [DataRow("TRue", 20)]
        [DataRow("   false   ", 17)]
        [DataRow("1", 17)]
        [DataRow("0", 17)]
        [DataRow("-1", 17)]
        [DataRow("-100", 17)]
        [DataRow("yes", 17)]
        [DataRow("no", 17)]
        [DataRow("on", 17)]
        [DataRow("off", 17)]
        public void UsersWithSuperUserFilter(string superUserFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
            {
                IncludeSystemAdmins = superUserFilter
            });
            //usersDataSource.IncludeSystemAdmins = superUserFilter;
            //usersDataSource.Configuration.Values[nameof(usersDataSource.IncludeSystemAdmins)] = superUserFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("1,2,3,4,5,6,7,8,9,10", "00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", "2", "9", true, 6)]
        // TODO: this test doesn't seem to do much different than the first?
        [DataRow("1,2,3,4,5,6,7,8,9,10", "00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", "1,2", "9", false, 6)]
        public void UsersWithAllFilters(string includeUsersFilter, string excludeUsersFilter, string includeRolesFilter, string excludeRolesFilter, bool superUserFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource(new
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
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        private Users GenerateUsersDataSourceDataSource(object options = default)
            => CreateDataSourceNew<Users>(new DataSourceOptions.Converter()
                .Create(new DataSourceOptions(lookUp: new LookUpTestData(GetService<DataBuilder>()).AppSetAndRes()), options));
    }
}