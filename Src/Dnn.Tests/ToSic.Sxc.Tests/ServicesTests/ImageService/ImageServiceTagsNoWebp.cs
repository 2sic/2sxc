using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Run;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceTagsNoWebp : ImageServiceTagsBase
    {
        // Start the test with a platform-info that has WebP support
        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services).AddTransient<IPlatformInfo, TestPlatformNoLicense>();
        }

        // [DataRow(ImgTagJpg, SrcSetNone, "No Src Set")]
        [DataRow(ImgTagJpg, SrcSet12, "With Src Set 1,2")]
        [TestMethod]
        public void ImgTagsMultiTests(string expected, string srcSet, string name) => ImageTagMultiTest(expected, srcSet, name);

        //[DataRow(SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
        //[DataRow(SrcJpgNone, SrcSetNone, false, "No Src Set, in-settings")]
        //[DataRow(SrcJpg12, SrcSet12, true, "With Src Set 1,2, in-pic")]
        //[DataRow(SrcJpg12, SrcSet12, false, "With Src Set 1,2, in-settings")]
        //[TestMethod]
        //public void SourceTags(string expected, string srcSet, bool inPicTag, string name) => SourceTagsInner(expected, srcSet, inPicTag, name);

        [DataRow(SrcJpgNone, SrcSetNone, "No Src Set")]
        [DataRow(SrcJpg12, SrcSet12, "With Src Set 1,2")]
        [TestMethod]
        public void SourceTagsMultiTests(string expected, string srcSet, string name) => SourceTagsMultiTest(expected, srcSet, name);


        [DataRow(SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
        [DataRow(SrcJpgNone, SrcSetNone, false, "No Src Set, in-settings")]
        [DataRow(SrcJpg12, SrcSet12, true, "With Src Set 1,2, in-pic")]
        [DataRow(SrcJpg12, SrcSet12, false, "With Src Set 1,2, in-settings")]
        [TestMethod]
        public void PictureTags(string expected, string srcSet, bool inPicTag, string name) 
            => PictureTagInner(expected, srcSet, inPicTag, name);
    }
}
