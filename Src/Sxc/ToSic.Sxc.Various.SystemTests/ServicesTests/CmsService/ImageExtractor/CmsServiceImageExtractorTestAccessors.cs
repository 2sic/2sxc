using ToSic.Sxc.Adam;
using ToSic.Sxc.Services.Cms.Sys;
using static ToSic.Sxc.Services.Cms.Sys.CmsServiceImageExtractor;

namespace ToSic.Sxc.ServicesTests.CmsService.ImageExtractor;

internal static class CmsServiceImageExtractorTestAccessors
{
    public static string GetImgServiceResizeFactorTac(string classAttribute)
        => GetImgServiceResizeFactor(classAttribute);

    public static string GetPictureClassesTac(string classAttribute)
    => GetPictureClasses(classAttribute);

    public static bool UseLightboxTac(string classAttribute)
        => UseLightbox(classAttribute);

    public static ImagePropertiesExtracted ExtractImagePropertiesTac( this CmsServiceImageExtractor extractor, string imgTag, IFolder folder)
        => extractor.ExtractImageProperties(imgTag, folder);

}