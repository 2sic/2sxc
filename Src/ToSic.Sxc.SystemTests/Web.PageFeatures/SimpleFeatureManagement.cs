using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Web.PageFeatures;

[Startup(typeof(StartupSxcCoreOnly))]
public class SimpleFeatureManagement(IPageFeaturesManager fm, PageFeaturesCatalog cat)//: TestBaseSxcDb
{
    [Fact]
    public void PageFeaturesCaseInsensitive()
    {
        //var fm = GetService<IPageFeaturesManager>();
        NotNull(fm.Features["turnOn"]);
        NotNull(fm.Features["Turnon"]);
    }
    [Fact]
    public void AdditionalFeatures()
    {
        //var fm = GetService<IPageFeaturesManager>();
        False(fm.Features.TryGetValue("dummy", out _));

        //var cat = GetService<PageFeaturesCatalog>();
        cat.Register(new PageFeature { NameId = "dummy", Name = "dummy-feature" });

        NotNull(fm.Features["dummy"]);

    }
}