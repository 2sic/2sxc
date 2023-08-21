using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    [TestClass]
    public class AsConverterFakeTests: AsConverterTestsBase
    {
        [TestMethod]
        public void EntityFake()
        {
            var fake = Cdf.FakeEntity(0);
            Assert.IsNotNull(fake);
            Assert.AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.EntityId);
            Assert.AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.RepositoryId);
        }

        [TestMethod]
        public void ItemFake()
        {
            var fake = Cdf.AsItem(Cdf.FakeEntity(0), noParamOrder: Protector, propsRequired: false);
            Assert.IsNotNull(fake);
            Assert.IsNull(fake.String("some-field"));
        }
    }
}
