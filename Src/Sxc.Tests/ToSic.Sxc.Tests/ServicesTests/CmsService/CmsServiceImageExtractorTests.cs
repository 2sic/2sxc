using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Internal.HtmlParsing;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests.CmsService;

[TestClass]
public class CmsServiceImageExtractorTests
{
    [TestMethod]
    [DataRow("css", null)]
    [DataRow("wysiwyg-75", "3/4")]
    [DataRow("wysiwyg-width of5", null)]
    [DataRow("class1   WYSIWYG-66 class3 wysiwyg-width1of5", "2/3")]
    [DataRow("wysiwyg-7", "7", DisplayName = "odd unprepared number")]
    [DataRow("wysiwyg-7", "7", DisplayName = "odd unprepared number")]
    public void GetFactor(string classAttribute, string expectedFactor)
        => AreEqual(expectedFactor, CmsServiceTestAccessors.TacGetImgServiceResizeFactor(classAttribute));

    [TestMethod]
    [DataRow("<p>some html</p>", 0)]
    [DataRow("<p><img bbb='cccccc'><img/><IMG aaa data-cmsid='xxx'/><iMG     data-cmsid='yyy'  QQQ=\"abc\"  ></p>", 2)]
    public void ImagesWithDataCmsid(string html, int matches)
        => AreEqual(matches, RegexUtil.ImagesDetection.Value.Matches(html).Count);

    [TestMethod]
    [DataRow("wysiwyg-lightbox", true)]
    [DataRow("wysiwyg-lightbox other-class", true)]
    [DataRow("other-class wysiwyg-lightbox", true)]
    [DataRow("other-class", false)]
    [DataRow(null, false)]
    public void UseLightbox(string classAttribute, bool expected)
        => AreEqual(expected, CmsServiceTestAccessors.TacUseLightbox(classAttribute));
}