using static ToSic.Sxc.Services.CmsService.CmsServiceImageExtractor;

namespace ToSic.Sxc.Tests.ServicesTests.CmsService;

internal class CmsServiceTestAccessors
{
    public static string TacGetImgServiceResizeFactor(string classAttribute)
        => GetImgServiceResizeFactor(classAttribute);

    public static string TacGetPictureClasses(string classAttribute)
    => GetPictureClasses(classAttribute);

    public static bool TacUseLightbox(string classAttribute)
        => UseLightbox(classAttribute);

}