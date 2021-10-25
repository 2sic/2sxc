using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Tests.DynamicData;
using ToSic.Sxc.Tests.WebTests.LinkHelperTests;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests.WebTests.LinkImageTests
{
    public class LinkImageTestBase: EavTestBase
    {
        public LinkImageTestBase()
        {
        }

        public ImgResizeLinker GetLinker() => Resolve<ImgResizeLinker>();
        public ILinkHelper GetLinkHelper() => Resolve<ILinkHelper>();

        public static DynamicReadObject ToDyn(object contents) => TestAccessors.DynReadObjT(contents, false, false);


        protected void EqualOnLinkerAndHelper(string expected,
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

            var linkHelper = GetLinkHelper();
            var helperResult = linkHelper.TestImage(url: url, settings: settings, factor: factor, width: width,
                height: height,
                quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
                aspectRatio: aspectRatio);
            Assert.AreEqual(expected, helperResult, "Failed on ILinkHelper");



        }
    }
}
