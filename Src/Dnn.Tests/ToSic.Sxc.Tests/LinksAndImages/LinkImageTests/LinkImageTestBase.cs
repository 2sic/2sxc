using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images;
using ToSic.Sxc.Tests.DynamicData;
using ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    public class LinkImageTestBase: TestBaseSxc
    {

        public ImgResizeLinker GetLinker() => Build<ImgResizeLinker>();
        public ILinkHelper GetLinkHelper() => Build<ILinkHelper>();

        public static DynamicReadObject ToDyn(object contents) => TestAccessors.DynReadObjT(contents, false, false);


        protected void TestOnLinkerAndHelper(string expected,
            string url = null,
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null)
        {
            // Test with Linker
            var linker = GetLinker();
            var linkerResult = linker.Image(url: url, settings: settings, factor: factor, width: width, height: height,
                quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
                aspectRatio: aspectRatio);
            Assert.AreEqual(expected, linkerResult, "Failed on ImgResizeLinker");

            // Skip Helper-tests if using SrcSet as that's not supported in that case
            // Because it would lead to not-expected result
            //if (srcset != null) return;

            var linkHelper = GetLinkHelper();
            var helperResult = linkHelper.TestImage(url: url, settings: settings, factor: factor, width: width,
                height: height,
                quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
                aspectRatio: aspectRatio);
            Assert.AreEqual(expected, helperResult, "Failed on ILinkHelper");
        }


        protected void TestOnLinkerSrcSet(string expected,
            string url = null,
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string srcset = null)
        {
            // Test with Linker
            var linker = GetLinker();
            var typedSettings = linker.ResizeParamMerger.BuildResizeSettings(settings: settings, factor: factor, width: width,
                height: height,
                quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
                aspectRatio: aspectRatio, srcset: srcset);
            var linkerResult = linker.SrcSet(url, typedSettings);
            Assert.AreEqual(expected, linkerResult, $"Failed on ImgResizeLinker for srcSet '{srcset}'");
        }
    }
}
