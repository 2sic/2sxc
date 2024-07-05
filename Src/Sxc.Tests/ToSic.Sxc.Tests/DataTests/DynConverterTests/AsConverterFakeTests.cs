using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    [TestClass]
    public class AsConverterFakeTests: AsConverterTestsBase
    {
        [TestMethod]
        public void EntityFake()
        {
            var fake = Cdf.TacFakeEntity(0);
            Assert.IsNotNull(fake);
            Assert.AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.EntityId);
            Assert.AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.RepositoryId);
        }

        [TestMethod]
        public void ItemFake()
        {
            var fake = Cdf.AsItem(Cdf.TacFakeEntity(0), propsRequired: false);
            Assert.IsNotNull(fake);
            Assert.IsNull(fake.String("some-field"));
        }
    }
}
