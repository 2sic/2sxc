using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests;

[TestClass]
public class AsConverterFakeTests: TestBaseSxcDb
{
    public CodeDataFactory Cdf => field ??= GetService<CodeDataFactory>();

    [TestMethod]
    public void EntityFake()
    {
        var fake = Cdf.FakeEntityTac(0);
        Assert.IsNotNull(fake);
        Assert.AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.EntityId);
        Assert.AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.RepositoryId);
    }

    [TestMethod]
    public void ItemFake()
    {
        var fake = Cdf.AsItem(Cdf.FakeEntityTac(0), propsRequired: false);
        Assert.IsNotNull(fake);
        Assert.IsNull(fake.String("some-field"));
    }
}