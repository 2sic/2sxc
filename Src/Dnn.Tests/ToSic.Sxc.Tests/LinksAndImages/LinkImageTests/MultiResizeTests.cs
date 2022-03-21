using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;
using static ToSic.Sxc.Tests.DataForImageTests.ResizeRecipesData;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class MultiResizeTests: LinkImageTestBase
    {
        [TestMethod]
        public void NoFactorMap()
        {
            var l = GetLinker();
            var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000);
            var f1 = l.DimGen.ResizeDimensions(settings, settings.Find(SrcSetType.Img, true));
            Assert.AreEqual(1000, f1.Width);

            var f2 = new ResizeSettings(settings, factor: 0.5);
            var dims = l.DimGen.ResizeDimensions(f2, f2.Find(SrcSetType.Img, true));
            Assert.AreEqual(500, dims.Width);
        }

        [DataRow(W75Alt, 0.75, "0.75 should be changed")]
        [DataRow(W100, 1, "1 should be changed too")]
        [DataRow(W50, 0.5, "0.5 should be changed")]
        [DataRow(700, 0.70, "0.70 should not be changed")]
        [DataRow((int)(1000 * .9), 0.9, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMap(int expected, double factor, string name) 
            => WithFactorMapInternal(expected, factor, name, TestRecipeSet());



        [DataRow(W100, 1, "1 should be changed too")]
        [DataRow(W50, 0.5, "0.5 should be changed")]
        [DataRow(W75Alt, 0.75, "0.75 should be changed")]
        [DataRow(700, 0.70, "0.70 should not be changed")]
        [DataRow((int)(1000 * .9), 0.9, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMapJson(int expected, double factor, string name)
        {
            // Test with json structure
            var factorsJson = JsonRecipe();
            WithFactorMapInternal(expected, factor, name + "-json", factorsJson);
        }


        private void WithFactorMapInternal(int expected, double factor, string name, object recipes)
        {
            var l = GetLinker();
            var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000, advanced: recipes);

            // Normally UseFactorMap is false because we set the width explicitly
            // But to run the test, we must set it to true
            settings.UseFactorMap = true;

            settings = new ResizeSettings(settings, factor: factor);
            var srcSetSettings = settings.Find(SrcSetType.Img, true);
            var f1 = l.DimGen.ResizeDimensions(settings, srcSetSettings);
            Assert.AreEqual(expected, f1.Width, name);
        }
    }
}
