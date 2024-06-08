using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void BasicCmsService(string html, string expectedHtml)
            => AreEqual(expectedHtml, CmsServiceShow(html).ToString());


    }
}
