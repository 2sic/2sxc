using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Configuration;
using BuiltInFeatures = ToSic.Eav.Internal.Features.BuiltInFeatures;

#pragma warning disable 618

namespace ToSic.Dnn.Tests.ToSic.Eav.Configuration.Features_Compatibility
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class FeaturesStaticTests: FeatureTestsBase
    {

        [TestMethod]
        [Ignore("I believe the setup doesn't work yet - needs to first load the licenses to be able to test this")]
        public void PasteClipboardActive()
        {
            var x = Features.Enabled(BuiltInFeatures.PasteImageFromClipboard.Guid);
            Assert.IsTrue(x, "this should be enabled and non-expired");
        }

        [TestMethod]
        [Ignore("I believe the setup doesn't work yet - needs to first load the licenses to be able to test this")]
        public void InventedFeatureGuid()
        {
            var inventedGuid = new Guid("12345678-1c8b-4286-a33b-3210ed3b2d9a");
            var x = Features.Enabled(inventedGuid);
            Assert.IsFalse(x, "this should be enabled and expired");
        }
    }
}
