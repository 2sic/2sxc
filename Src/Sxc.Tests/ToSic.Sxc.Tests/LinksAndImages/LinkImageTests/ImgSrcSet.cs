using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class ImgSrcSet : LinkImageTestBase
    {
        [DataRow("test.jpg")]
        [DataRow("test.png")]
        [DataRow("/test.jpg")]
        [DataRow("//test.jpg")]
        [DataRow("http://www.2sxc.org/test.jpg")]
        [DataRow("weird-extension.abc")]
        [DataTestMethod]
        public void EmptySrcSet(string url) 
            => TestOnLinkerSrcSet(url, url, variants: "d");

        [DataRow("test.jpg?w=1000 1000w", "test.jpg", "1000")]
        [DataRow("test.jpg?w=1000 1000w", "test.jpg", "1000w")]
        [DataRow("test.jpg?w=1000 1000w,\ntest.jpg?w=2000 2000w", "test.jpg", "1000,2000")]
        [DataRow("test.jpg?w=500 500w,\ntest.jpg?w=1000 1000w,\ntest.jpg?w=2000 2000w", "test.jpg", "500w,1000w,2000w")]
        [DataTestMethod]
        public void SrcSetUrlOnlyW(string expected, string url, string variants) 
            => TestOnLinkerSrcSet(expected, url, variants: variants);




        [DataRow("test.jpg 1x", "test.jpg", "1")]
        [DataRow("test.jpg 1.5x", "test.jpg", "1.5x")]
        [DataTestMethod]
        public void SrcSetUrlOnlyX(string expected, string url, string variants) 
            => TestOnLinkerSrcSet(expected, url, variants: variants);

        [DataRow("test.jpg?w=1800 1.5x", "test.jpg", "1.5x")]
        [DataRow("test.jpg?w=1200 1x", "test.jpg", "1")]
        [DataRow("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2400 2x", "test.jpg", "1x,1.5x,2")]
        [DataRow("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2000 2x", "test.jpg", "1x,1.5x,2x=2000")]
        [DataRow("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2000&h=1000 2x", "test.jpg", "1x,1.5x,2x=2000:1000")]
        [DataTestMethod]
        public void SrcSetUrlXAndWidth(string expected, string url, string variants)
            => TestOnLinkerSrcSet(expected, url, width: 1200, variants: variants);




        [DataRow("test.jpg?w=1200 1200w", "test.jpg", "1*")]
        [DataRow("test.jpg?w=1800 1800w", "test.jpg", "1.5*")]
        [DataRow("test.jpg?w=600 600w", "test.jpg", "0.5*")]
        [DataRow("test.jpg?w=600 600w", "test.jpg", "1:2*")]
        [DataRow("test.jpg?w=600 600w", "test.jpg", "1/2*")]
        [DataRow("test.jpg?w=600 600w", "test.jpg", "1/2")] // without '*' it auto-detects a proportion
        [DataRow("test.jpg?w=600 600w", "test.jpg", "1:2")] // without '*' it auto-detects a proportion
        [DataTestMethod]
        public void SrcSetUrlOnlyStar(string expected, string url, string variants) 
            => TestOnLinkerSrcSet(expected, url, variants: variants);

        [DataRow("test.jpg?w=120 120w", "test.jpg", "1*")]
        [DataRow("test.jpg?w=180 180w", "test.jpg", "1.5*")]
        [DataRow("test.jpg?w=120 120w,\ntest.jpg?w=180 180w,\ntest.jpg?w=240 240w", "test.jpg", "1*,1.5*,2*")]
        // Note: These two cases don't make sense but could be configured - as soon as you give an exact pixel, the 2* doesn't do much
        [DataRow("test.jpg?w=120 120w,\ntest.jpg?w=200 200w", "test.jpg", "1*,2*=200")]
        [DataRow("test.jpg?w=120 120w,\ntest.jpg?w=200&h=180 200w", "test.jpg", "1*,2*=200:180")]
        [DataTestMethod]
        public void SrcSetUrlStarAndWidth(string expected, string url, string variants)
            => TestOnLinkerSrcSet(expected, url, width: 120, variants: variants);


        [TestMethod]
        public void WipDoubleResize()
        {
            var linker = GetLinker();
            var settings = linker.ResizeParamMerger.BuildResizeSettings(width: 1000, factor: 0.5, advanced: AdvancedSettings.Parse("0.5"));
            var src = linker.SrcSet("test.jpg", settings, SrcSetType.Img);
            Assert.AreEqual("test.jpg?w=250 250w", src, "Src should be a quarter now");

        }
    }
}
