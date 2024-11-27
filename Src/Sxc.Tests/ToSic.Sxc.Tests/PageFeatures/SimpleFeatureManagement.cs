using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Tests.PageFeatures
{
    [TestClass]
    public class SimpleFeatureManagement: TestBaseSxcDb
    {
        [TestMethod]
        public void PageFeaturesCaseInsensitive()
        {
            var fm = GetService<IPageFeaturesManager>();
            Assert.IsTrue(fm.Features["turnOn"] != null);
            Assert.IsTrue(fm.Features["Turnon"] != null);
        }
        [TestMethod]
        public void AdditionalFeatures()
        {
            var fm = GetService<IPageFeaturesManager>();
            Assert.IsTrue(!fm.Features.TryGetValue("dummy", out _));

            var cat = GetService<PageFeaturesCatalog>();
            cat.Register(new PageFeature { NameId = "dummy", Name = "dummy-feature" });

            Assert.IsTrue(fm.Features["dummy"] != null);

        }
    }
}
