using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Core.Tests.LookUp;
using ToSic.Eav.DataSourceTests;
using ToSic.Razor.Html5;
using ToSic.Sxc.DataSources;
using ToSic.Testing.Shared;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Tests.DataSources
{
    [TestClass()]
    public class UsersDataSourceTests : TestBaseDiEavFullAndDb
    {
        [TestMethod()]
        public void UsersDefault()
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            Assert.AreEqual(20, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-integer, -1", 20)]
        [DataRow("1", 1)]
        [DataRow("2,3", 2)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 3)]
        public void UsersWithIncludeUserIdsFilter(string includeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.IncludeUsersFilter = includeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", 20)]
        [DataRow("00000000-0000-0000-0000-000000000001", 1)]
        [DataRow("00000000-0000-0000-0000-000000000002,00000000-0000-0000-0000-000000000003", 2)]
        [DataRow("a,b,c,-2,-1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 3)]
        public void UsersWithIncludeUserGuidsFilter(string includeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.IncludeUsersFilter = includeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-integer,-1", 20)]
        [DataRow("1", 19)]
        [DataRow("2,3", 18)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 17)]
        public void UsersWithExcludeUserIdsFilter(string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.ExcludeUsersFilter = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 20)]
        [DataRow("not-a-guid, 00000000-0000-0000-0000-000000000000", 20)]
        [DataRow("00000000-0000-0000-0000-000000000001", 19)]
        [DataRow("00000000-0000-0000-0000-000000000002,00000000-0000-0000-0000-000000000003", 18)]
        [DataRow("a,b,c,-2,-1,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000004,00000000-0000-0000-0000-000000000005,00000000-0000-0000-0000-000000000006,00000000-0000-0000-0000-000000000004", 17)]
        public void UsersWithExcludeUserGuidsFilter(string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.ExcludeUsersFilter = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", "", 20)]
        [DataRow("not-a-integer,-1", "not-a-integer,-1", 20)]
        [DataRow("1,2,3", "1,2", 1)]
        [DataRow("3,4,5", "1,2,3", 2)] // TODO: stv# this is only part....
        public void UsersWithIncludeExcludeFilter(string includeUsersFilter, string excludeUsersFilter, int expected)
        {
            var usersDataSource = GenerateUsersDataSourceDataSource();
            usersDataSource.IncludeUsersFilter = includeUsersFilter;
            usersDataSource.ExcludeUsersFilter = excludeUsersFilter;
            Assert.AreEqual(expected, usersDataSource.List.ToList().Count);
        }


        // TEST cases
        //Configuration[ExcludeUsersFilterKey] = "d65e097e-afde-4a46-a8ab-9d3ed277b4a9,989358ab-86ad-44a7-8b35-412e076e469a";
        //Configuration[ExcludeUsersFilterKey] = "d65e097e-afde-4a46-a8ab-9d3ed277b4a9";
        //Configuration[ExcludeUsersFilterKey] = "not-a-guid";
        //Configuration[ExcludeRolesFilterKey] = "1096,1097,1101,1102,1103";
        //Configuration[ExcludeRolesFilterKey] = "1102,1103";
        //Configuration[ExcludeRolesFilterKey] = "1101";
        //Configuration[ExcludeRolesFilterKey] = "not-a-integer,-1";

        private MockUsersDataSource GenerateUsersDataSourceDataSource() 
            => this.GetTestDataSource<MockUsersDataSource>(LookUpTestData.AppSetAndRes());
    }
}