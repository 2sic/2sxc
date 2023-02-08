using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ToSic.Eav.Core.Tests.LookUp;
using ToSic.Eav.DataSourceTests;
using ToSic.Sxc.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources
{
    [TestClass()]
    public class UsersDataSourceTests : TestBaseSxcDb
    {
        // Start the test with a platform-info that has a patron
        protected override IServiceCollection SetupServices(IServiceCollection services)
        {
            return base.SetupServices(services).AddTransient<UsersDataSourceProvider, MockUsersDataSource>(); ;
        }


        [TestMethod()]
        public void UsersDefault()
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            Assert.AreEqual(20, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-integer, -1", 20)]
        [DataRow("1", 0)]
        [DataRow("2,3", 0)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 3)]
        public void UsersWithIncludeUserIdsFilter(string includeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.UserIds = includeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", 20)]
        [DataRow("00000000-0000-0000-0000-000000000001", 0)]
        [DataRow("00000000-0000-0000-0000-000000000002,00000000-0000-0000-0000-000000000003", 0)]
        [DataRow("a,b,c,-2,-1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 3)]
        public void UsersWithIncludeUserGuidsFilter(string includeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.UserIds = includeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-integer,-1", 20)]
        [DataRow("1", 20)]
        [DataRow("2,3", 20)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 17)]
        public void UsersWithExcludeUserIdsFilter(string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.ExcludeUserIds = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", 20)]
        [DataRow("00000000-0000-0000-0000-000000000001", 20)]
        [DataRow("00000000-0000-0000-0000-000000000002,00000000-0000-0000-0000-000000000003", 20)]
        [DataRow("a,b,c,-2,-1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 17)]
        public void UsersWithExcludeUserGuidsFilter(string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.ExcludeUserIds = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", "not-a-integer,-1", 20)]
        [DataRow("3,4,5", "00000000-0000-0000-0000-000000000001, 00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", 2)]
        [DataRow("00000000-0000-0000-0000-000000000001, 00000000-0000-0000-0000-000000000002,00000000-0000-0000-0000-000000000003", "2,3", 0)]
        public void UsersWithIncludeExcludeUsersFilter(string includeUsersFilter, string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.UserIds = includeUsersFilter;
            usersDataSource.ExcludeUserIds = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-integer,-1", 0)]
        [DataRow("9", 14)]
        [DataRow("1,2", 7)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 3)]
        public void UsersWithIncludeRolesFilter(string includeRolesFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.RoleIds = includeRolesFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-integer,-1", 20)]
        [DataRow("9", 6)]
        [DataRow("1,2", 13)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 17)]
        public void UsersWithExcludeRolesFilter(string excludeRolesFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.ExcludeRoleIds = excludeRolesFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("1,10", "3", 13)]
        public void UsersWithIncludeExcludeRolesFilter(string includeRolesFilter, string excludeRolesFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.RoleIds = includeRolesFilter;
            usersDataSource.ExcludeRoleIds = excludeRolesFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 23)]
        [DataRow("not-a-bool", 23)]
        [DataRow("true", 3)]
        [DataRow("false", 20)]
        [DataRow("TRue", 3)]
        [DataRow("   false   ", 20)]
        [DataRow("1", 23)]
        [DataRow("0", 23)]
        [DataRow("-1", 23)]
        [DataRow("-100", 23)]
        [DataRow("yes", 23)]
        [DataRow("no", 23)]
        [DataRow("on", 23)]
        [DataRow("off", 23)]
        public void UsersWithSuperUserFilter(string superUserFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.IncludeSystemAdmins = superUserFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("1,2,3,4,5,6,7,8,9,10", "00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", "2", "9", "", 6)]
        [DataRow("1,2,3,4,5,6,7,8,9,10", "00000000-0000-0000-0000-000000000002, 00000000-0000-0000-0000-000000000003", "1,2", "9", "true", 1)]
        public void UsersWithAllFilters(string includeUsersFilter, string excludeUsersFilter, string includeRolesFilter, string excludeRolesFilter, string superUserFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.UserIds = includeUsersFilter;
            usersDataSource.ExcludeUserIds = excludeUsersFilter;
            usersDataSource.RoleIds = includeRolesFilter;
            usersDataSource.ExcludeRoleIds = excludeRolesFilter;
            usersDataSource.IncludeSystemAdmins = superUserFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        private Users GenerateUsersDataSourceDataSource() 
            => this.GetTestDataSource<Users>(LookUpTestData.AppSetAndRes());
    }
}