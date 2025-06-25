using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.PageFeatures;

namespace ToSic.Sxc.Web.PageFeatures;

[Startup(typeof(StartupSxcCoreOnly))]
public class SimpleFeatureManagement(IPageFeaturesManager fm, PageFeaturesCatalog cat)
{
    [Fact]
    public void PageFeaturesCaseInsensitive()
    {
        NotNull(fm.Features["turnOn"]);
        NotNull(fm.Features["Turnon"]);
    }
    [Fact]
    public void AdditionalFeatures()
    {
        False(fm.Features.TryGetValue("dummy", out _));

        cat.Register(new PageFeature { NameId = "dummy", Name = "dummy-feature" });

        NotNull(fm.Features["dummy"]);

    }
}