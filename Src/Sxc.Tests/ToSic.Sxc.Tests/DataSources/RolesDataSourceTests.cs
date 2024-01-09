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
    [TestClass()]
    public class RolesDataSourceTests : TestBaseSxcDb
    {
        // Start the test with a platform-info that has a patron
        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddTransient<RolesDataSourceProvider, MockRolesDataSource>();
        }
        [TestMethod()]
        public void RolesDefault()
        {
            var rolesDataSource = GenerateRolesDataSourceDataSource();
            Assert.AreEqual(10, rolesDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 10)]
        [DataRow("not-a-integer,-1", 0)]
        [DataRow("1", 1)]
        [DataRow("2,3", 2)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 3)]
        public void RolesWithIncludeFilter(string includeRolesFilter, int expected)
        {
            var rolesDataSource = GenerateRolesDataSourceDataSource(new
            {
                RoleIds = includeRolesFilter
            });
            //rolesDataSource.RoleIds = includeRolesFilter;
            Assert.AreEqual(expected, rolesDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", 10)]
        [DataRow("not-a-integer,-1", 10)]
        [DataRow("1", 9)]
        [DataRow("2,3", 8)]
        [DataRow("a,b,c,-2,-1,4,4,5,6,4", 7)]
        public void RolesWithExcludeFilter(string excludeRolesFilter, int expected)
        {
            var rolesDataSource = GenerateRolesDataSourceDataSource(new
            {
                ExcludeRoleIds = excludeRolesFilter
            });
            //rolesDataSource.ExcludeRoleIds = excludeRolesFilter;
            Assert.AreEqual(expected, rolesDataSource.List.ToList().Count);
        }

        [DataTestMethod]
        [DataRow("", "", 10)]
        [DataRow("not-a-integer,-1", "not-a-integer,-1", 0)]
        [DataRow("3,4,5", "1,2,3", 2)]
        public void RolesWithIncludeExcludeFilter(string includeRolesFilter, string excludeRolesFilter, int expected)
        {
            var rolesDataSource = GenerateRolesDataSourceDataSource(new
            {
                RoleIds = includeRolesFilter,
                ExcludeRoleIds = excludeRolesFilter
            });
            //rolesDataSource.RoleIds = includeRolesFilter;
            //rolesDataSource.ExcludeRoleIds = excludeRolesFilter;
            Assert.AreEqual(expected, rolesDataSource.List.ToList().Count);
        }

        private Roles GenerateRolesDataSourceDataSource(object options = default) 
            => CreateDataSourceNew<Roles>(new DataSourceOptions.Converter()
                    .Create(new DataSourceOptions(lookUp: new LookUpTestData(GetService<DataBuilder>()).AppSetAndRes()), options));
    }
}