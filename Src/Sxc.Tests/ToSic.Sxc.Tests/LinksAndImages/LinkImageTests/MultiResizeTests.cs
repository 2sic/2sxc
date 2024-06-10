using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
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
            var f1 = l.DimGen.ResizeDimensions(settings, settings.Find(SrcSetType.Img, true, null));
            Assert.AreEqual(1000, f1.Width);

            var f2 = new ResizeSettings(settings, factor: 0.5);
            var dims = l.DimGen.ResizeDimensions(f2, f2.Find(SrcSetType.Img, true, null));
            Assert.AreEqual(500, dims.Width);
        }

        [DataRow(W50, 0.5, CssNone, "0.5 should be changed")]
        [DataRow(700, 0.70, CssNone, "0.70 should not be changed")]
        [DataRow(W75ImgOnly777, 0.75, CssNone, "0.75 should be changed")]
        [DataRow(W75CssUnknown, 0.75, CssUnknown, "0.75 should be changed")]
        [DataRow(W100, 1, CssNone, "1 should be changed too")]
        [DataRow((int)(1000 * .9), 0.9, CssNone, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMap(int expected, double factor, string cssFramework, string name) 
            => WithFactorMapInternal(expected, factor, cssFramework, TestRecipeSet(), name);



        [DataRow(W50, 0.5, CssNone, "0.5 should be changed")]
        [DataRow(700, 0.70, CssNone, "0.70 should not be changed")]
        [DataRow(W75ImgOnly777, 0.75, CssNone, "0.75 should be changed")]
        [DataRow(W75CssUnknown, 0.75, CssUnknown, "0.75 should be changed")]
        [DataRow(W100, 1, CssNone, "1 should be changed too")]
        [DataRow((int)(1000 * .9), 0.9, CssNone, "0.9 should just calculate, because it's not in the factor-list")]
        [DataTestMethod]

        public void WithFactorMapJson(int expected, double factor, string cssFramework, string name) 
            => WithFactorMapInternal(expected, factor, cssFramework, TestRecipeSetFromJson, name + "-json");


        private void WithFactorMapInternal(int expected, double factor, string cssFramework, AdvancedSettings recipes, string name)
        {
            var l = GetLinker();
            var settings = l.ResizeParamMerger.BuildResizeSettings(width: 1000, advanced: recipes);

            // Normally UseFactorMap is false because we set the width explicitly
            // But to run the test, we must set it to true
            settings.UseFactorMap = true;

            settings = new(settings, factor: factor);
            var srcSetSettings = settings.Find(SrcSetType.Img, true, cssFramework);
            var f1 = l.DimGen.ResizeDimensions(settings, srcSetSettings);
            Assert.AreEqual(expected, f1.Width, name);
        }
    }
}
