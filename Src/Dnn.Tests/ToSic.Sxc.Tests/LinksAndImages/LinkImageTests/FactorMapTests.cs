using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class FactorMapTests: LinkImageTestBase
    {
        private const int W100 = 990;
        private const int W75 = 700;
        private const int W50 = 450;
        private const int W25 = 200;

        [TestMethod]
        public void NoFactorMap()
        {
            var l = GetLinker();
            var settings = (ResizeSettings)l.ResizeParamMerger.BuildResizeSettings(width: 1000);
            var f1 = l.DimGen.ResizeDimensions(new ResizeSettings(settings));
            Assert.AreEqual(1000, f1.Width);

            var f2 = new ResizeSettings(settings, factor: 0.5);
            var dims = l.DimGen.ResizeDimensions(f2);
            Assert.AreEqual(500, dims.Width);
        }

        [DataRow(W100, 1, "1 should be changed too")]
        [DataRow(W50, 0.5, "0.5 should be changed")]
        [DataRow(W75, 0.75, "0.75 should be changed")]
        [DataRow(700, 0.70, "0.70 should not be changed")]
        [DataRow((int)(1000 * .9), 0.9, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMap(int expected, double factor, string name)
        {
            var adv = new ResizeSettingsAdvanced
            {
                FactorsImport = new Dictionary<string, FactorRule>
                {
                    { "1", new FactorRule { Width = W100 } },
                    { "3/4", new FactorRule { Width = W75 } },
                    { "1:2", new FactorRule { Width = W50 } },
                    { "0.25", new FactorRule { Width = W25 } }
                }
            };

            var l = GetLinker();
            var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000/*, advanced: adv*/);

            settings.Advanced = adv;


            var f1 = l.DimGen.ResizeDimensions(new ResizeSettings(settings, factor: factor));
            Assert.AreEqual(expected, f1.Width, name);
        }
    }
}
