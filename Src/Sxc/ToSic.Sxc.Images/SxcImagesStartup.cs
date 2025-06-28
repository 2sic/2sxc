using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcImagesStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcImages(this IServiceCollection services)
    {
        // new in v12.02/12.04 Image Link Resize Helper
        services.TryAddTransient<ImgResizeLinker>();
        services.TryAddTransient<IImageService, ImageService>();
        services.TryAddTransient<ResizeDimensionGenerator>();

        // New v20 ImageMetadataRecommendation
        services.TryAddTransient<IImageMetadataRecommendationsService, ImageMetadataRecommendationsService>();

        return services;
    }
}