using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.DataTests.DynConverterTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class AsConverterAsItem(ICodeDataFactory cdf)
{
    [Fact]
    public void AsItemWithFakeOk()
    {
        var item = cdf.AsItemTac(cdf.FakeEntityTac(0), propsRequired: true);
        NotNull(item);
    }

    [Fact]
    public void AsItemWithAnonFail()
    {
        var data = new
        {
            Title = "This is a title",
            Birthday = new DateTime(2012, 02, 07)
        };

        Throws<ArgumentException>(() =>
        {
            var item = cdf.AsItemTac(data, propsRequired: true);
        });
    }
}