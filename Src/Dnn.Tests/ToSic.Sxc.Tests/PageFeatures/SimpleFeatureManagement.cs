using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.PageFeatures;

namespace ToSic.Sxc.Tests.PageFeatures
{
    [TestClass]
    public class SimpleFeatureManagement: TestBaseSxcDb
    {
        [TestMethod]
        public void PageFeaturesCaseInsensitive()
        {
            var fm = Build<IPageFeaturesManager>();
            Assert.IsTrue(fm.Features["turnOn"] != null);
            Assert.IsTrue(fm.Features["Turnon"] != null);
        }
        [TestMethod]
        public void AdditionalFeatures()
        {
            var fm = Build<IPageFeaturesManager>();
            Assert.IsTrue(!fm.Features.TryGetValue("dummy", out _));

            var cat = Build<PageFeaturesCatalog>();
            cat.Register(new PageFeature("dummy", "dummy-feature"));

            Assert.IsTrue(fm.Features["dummy"] != null);

        }
    }
}
