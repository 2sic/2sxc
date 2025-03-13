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
        IsNotNull(fake);
        AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.EntityId);
        AreEqual(DataConstants.DataFactoryDefaultEntityId, fake.RepositoryId);
    }

    [TestMethod]
    public void ItemFake()
    {
        var fake = Cdf.AsItem(Cdf.FakeEntityTac(0), propsRequired: false);
        IsNotNull(fake);
        IsNull(fake.String("some-field"));
    }
}