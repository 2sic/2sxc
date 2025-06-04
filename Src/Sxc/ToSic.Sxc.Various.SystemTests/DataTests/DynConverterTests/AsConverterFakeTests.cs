using ToSic.Eav.Data;
using ToSic.Eav.Data.Sys;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.DataTests.DynConverterTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class AsConverterFakeTests(ICodeDataFactory cdf)
{
    [Fact]
    public void EntityFake()
    {
        var fake = cdf.FakeEntityTac(0);
        NotNull(fake);
        Equal(DataConstants.DataFactoryDefaultEntityId, fake.EntityId);
        Equal(DataConstants.DataFactoryDefaultEntityId, fake.RepositoryId);
    }

    [Fact]
    public void ItemFake()
    {
        var fake = cdf.AsItem(cdf.FakeEntityTac(0), propsRequired: false);
        NotNull(fake);
        Null(fake.String("some-field"));
    }
}