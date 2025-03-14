using static ToSic.Sxc.Services.CmsService.Internal.CmsServiceImageExtractor;

namespace ToSic.Sxc.ServicesTests.CmsService;

internal class CmsServiceTestAccessors
{
    public static string GetImgServiceResizeFactorTac(string classAttribute)
        => GetImgServiceResizeFactor(classAttribute);

    public static string GetPictureClassesTac(string classAttribute)
    => GetPictureClasses(classAttribute);

    public static bool UseLightboxTac(string classAttribute)
        => UseLightbox(classAttribute);

}