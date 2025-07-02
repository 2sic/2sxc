using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

internal static class ImageServiceTestAccessors
{

    public static IResizeSettings SettingsTac(this IImageService parent,
        object? settings = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakResize, ITweakResize>? tweak = null,
        object? factor = null,
        object? width = null,
        object? height = null,
        object? quality = null,
        string? resizeMode = null,
        string? scaleMode = null,
        string? format = null,
        object? aspectRatio = null,
        string? parameters = null,
        object? recipe = null
    ) => parent.Settings(settings: settings, noParamOrder: noParamOrder, tweak: tweak,
        factor: factor, width: width, height: height,
        quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
        parameters: parameters, recipe: recipe);

    public static IImageFormat GetFormatTac(this IImageService imageSvc, string path) =>
        imageSvc.GetFormat(path);

    public static IList<IImageFormat> GetResizeFormatTac(this IImageService imageSvc, string path) =>
        imageSvc.GetFormatTac(path).ResizeFormats;
}