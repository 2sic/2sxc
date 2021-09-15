using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.PageFeatures
{
    [TestClass]
    public class SimpleFeatureManagement
    {
        [TestMethod]
        public void AddFeatures()
        {
            var fm = new Sxc.Web.PageFeatures.PageFeaturesManager();

            //fm.Register(new PageFeature("turnOn", "turnOn JS"));

            Assert.IsTrue(fm.Features["turnOn"] != null);
            Assert.IsTrue(fm.Features["Turnon"] != null);
        }
    }
}
