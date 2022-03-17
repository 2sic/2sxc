using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class MultiResizeTests: LinkImageTestBase
    {
        private const int W100 = 990;
        private const int W75 = 700;
        private const int W75Alt = 777;
        private const int W50 = 450;
        private const int W25 = 200;

        [TestMethod]
        public void NoFactorMap()
        {
            var l = GetLinker();
            var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000);
            var f1 = l.DimGen.ResizeDimensions(settings, settings.Find(SrcSetType.ImgSrc));
            Assert.AreEqual(1000, f1.Width);

            var f2 = new ResizeSettings(settings, factor: 0.5);
            var dims = l.DimGen.ResizeDimensions(f2, f2.Find(SrcSetType.ImgSrc));
            Assert.AreEqual(500, dims.Width);
        }

        [DataRow(W100, 1, "1 should be changed too")]
        [DataRow(W50, 0.5, "0.5 should be changed")]
        [DataRow(W75Alt, 0.75, "0.75 should be changed")]
        [DataRow(700, 0.70, "0.70 should not be changed")]
        [DataRow((int)(1000 * .9), 0.9, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMap(int expected, double factor, string name)
        {
            // existing
            var adv = new MultiResizeSettings
            {
                Rules = new[]
                {
                    new MultiResizeRule { Factor = "1", Width = W100 },
                    new MultiResizeRule
                    {
                        Factor = "3/4",
                        Width = W75,
                        Sub = new[] { new MultiResizeRule { Type = "img", Width = W75Alt } }
                    },
                    new MultiResizeRule { Factor = "1:2", Width = W50 },

                    new MultiResizeRule { Factor = "0.25", Width = W25 }
                }
            };

            WithFactorMapInternal(expected, factor, name, adv);
        }

        [DataRow(W100, 1, "1 should be changed too")]
        [DataRow(W50, 0.5, "0.5 should be changed")]
        [DataRow(W75Alt, 0.75, "0.75 should be changed")]
        [DataRow(700, 0.70, "0.70 should not be changed")]
        [DataRow((int)(1000 * .9), 0.9, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMapJson(int expected, double factor, string name)
        {
            // Test with json structure
            var adv = new
            {
                rules = new object[]
                {
                    new { factor = "1", width = W100 },
                    new { factor = "3/4", width = W75, sub = new object[] { new { type = "img", width = W75Alt } } },
                    new { factor = "1:2", width = W50 },
                    new { factor = "0.25", width = W25 }
                }
            };
            var factorsJson = JsonConvert.SerializeObject(adv, Formatting.Indented);
            WithFactorMapInternal(expected, factor, name + "-json", factorsJson);
        }

        private void WithFactorMapInternal(int expected, double factor, string name, object adv)
        {
            var l = GetLinker();
            var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000, advanced: adv);

            // Normally UseFactorMap is false because we set the width explicitly
            // But to run the test, we must set it to true
            settings.UseFactorMap = true;

            settings = new ResizeSettings(settings, factor: factor);
            var srcSetSettings = settings.Find(SrcSetType.ImgSrc);
            var f1 = l.DimGen.ResizeDimensions(settings, srcSetSettings);
            Assert.AreEqual(expected, f1.Width, name);
        }
    }
}
