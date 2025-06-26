using ToSic.Sxc.Web.Sys.HtmlParsing;

namespace ToSic.Sxc.ServicesTests.CmsService.ImageExtractor;

public class ExtractorPartsTests
{
    [Theory]
    [InlineData("css", null)]
    [InlineData("wysiwyg-75", "3/4")]
    [InlineData("wysiwyg-width of5", null)]
    [InlineData("class1   WYSIWYG-66 class3 wysiwyg-width1of5", "2/3")]
    [InlineData("wysiwyg-7", "7")]//, DisplayName = "odd unprepared number")]
    public void GetFactor(string classAttribute, string expectedFactor)
        => Equal(expectedFactor, CmsServiceImageExtractorTestAccessors.GetImgServiceResizeFactorTac(classAttribute));

    [Theory]
    [InlineData("<p>some html</p>", 0)]
    [InlineData("<p><img bbb='cccccc'><img/><IMG aaa data-cmsid='xxx'/><iMG     data-cmsid='yyy'  QQQ=\"abc\"  ></p>", 2)]
    public void ImagesWithDataCmsid(string html, int matches)
        => Equal(matches, RegexUtil.ImagesDetection.Value.Matches(html).Count);

    [Theory]
    [InlineData("wysiwyg-lightbox", true)]
    [InlineData("wysiwyg-lightbox other-class", true)]
    [InlineData("other-class wysiwyg-lightbox", true)]
    [InlineData("other-class", false)]
    [InlineData(null, false)]
    public void UseLightbox(string classAttribute, bool expected)
        => Equal(expected, CmsServiceImageExtractorTestAccessors.UseLightboxTac(classAttribute));
}