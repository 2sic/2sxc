using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Images;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcImagesStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcImages(this IServiceCollection services)
    {
        // new in v12.02/12.04 Image Link Resize Helper
        services.TryAddTransient<ImgResizeLinker>();
        services.TryAddTransient<IImageService, ImageService>();

        return services;
    }


        
}