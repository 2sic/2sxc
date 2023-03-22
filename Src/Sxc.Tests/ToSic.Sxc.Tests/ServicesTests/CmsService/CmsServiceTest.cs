using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Utils;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests.CmsService
{
    [TestClass]
    public class CmsServiceTest : CmsServiceTestBase
    {
        [TestMethod]
        [DataRow(null, "")]
        [DataRow("", "")]
        [DataRow("<p>some html</p>", "<p>some html</p>")]
        [DataRow("<img src='img.png' data-cmsid='file:1' class='wysiwyg-width1of5'>",
            "<picture><source type='' srcset='http://mock.converted/file:1'><img src='http://mock.converted/file:1' class='wysiwyg-width1of5'></picture>")]
        [DataRow("<img src='tst.jpg' data-cmsid='file:1' class='img-fluid' loading='lazy' alt='description' width='1230' height='760' style='width:auto;'>",
            "<picture><source type='' srcset='http://mock.converted/file:1?w=1230'><img src='http://mock.converted/file:1?w=1230' alt='description' class='img-fluid' loading='lazy' height='760' style='width:auto;'></picture>")]
        public void BasicCmsService(string html, string expectedHtml) => AreEqual(expectedHtml, CmsServiceShow(html).ToString());

        [TestMethod]
        [DataRow("css", null)]
        [DataRow("wysiwyg-width1of5", "1/5")]
        [DataRow("wysiwyg-width of5", null)]
        [DataRow("class1   WYSIWYG-WIDTH2OF3 class3 wysiwyg-width1of5", "2/3")]
        public void GetFactor(string classAttribute, string expectedFactor) => AreEqual(expectedFactor, Services.CmsService.CmsServiceImageExtractor.GetImgServiceResizeFactor(classAttribute));

        [TestMethod]
        [DataRow("<p>some html</p>", 0)]
        [DataRow("<p><img bbb='cccccc'><img/><IMG aaa data-cmsid='xxx'/><iMG     data-cmsid='yyy'  QQQ=\"abc\"  ></p>", 2)]
        public void ImagesWithDataCmsid(string html, int matches) => AreEqual(matches, RegexUtil.ImagesDetection.Value.Matches(html).Count);
    }
}
