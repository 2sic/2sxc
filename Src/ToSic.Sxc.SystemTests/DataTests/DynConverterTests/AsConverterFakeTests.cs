using ToSic.Eav.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class AsConverterFakeTests(CodeDataFactory Cdf)//: TestBaseSxcDb
{
    //public CodeDataFactory Cdf => field ??= GetService<CodeDataFactory>();

    [Fact]
    public void EntityFake()
    {
        var fake = Cdf.FakeEntityTac(0);
        NotNull(fake);
        Equal(DataConstants.DataFactoryDefaultEntityId, fake.EntityId);
        Equal(DataConstants.DataFactoryDefaultEntityId, fake.RepositoryId);
    }

    [Fact]
    public void ItemFake()
    {
        var fake = Cdf.AsItem(Cdf.FakeEntityTac(0), propsRequired: false);
        NotNull(fake);
        Null(fake.String("some-field"));
    }
}