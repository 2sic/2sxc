//namespace ToSic.Sxc.Tests.ServicesTests.CmsService;

//public class CmsServiceTest : CmsServiceTestBase
//{
//    [Theory]
//    [InlineData(null, "")]
//    [InlineData("", "")]
//    [InlineData("<p>some html</p>", "<p>some html</p>")]
//    [InlineData("<img src='img.png' data-cmsid='file:1' class='wysiwyg-width1of5'>",
//        "<picture><source type='' srcset='http://mock.converted/file:1'><img src='http://mock.converted/file:1' class='wysiwyg-width1of5'></picture>")]
//    [InlineData("<img src='tst.jpg' data-cmsid='file:1' class='img-fluid' loading='lazy' alt='description' width='1230' height='760' style='width:auto;'>",
//        "<picture><source type='' srcset='http://mock.converted/file:1?w=1230'><img src='http://mock.converted/file:1?w=1230' alt='description' class='img-fluid' loading='lazy' height='760' style='width:auto;'></picture>")]
//    public void BasicCmsService(string html, string expectedHtml)
//        => Equal(expectedHtml, CmsServiceShow(html).ToString());


//}