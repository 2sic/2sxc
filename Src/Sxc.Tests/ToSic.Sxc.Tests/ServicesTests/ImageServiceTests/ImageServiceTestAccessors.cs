using System;
using System.Collections.Generic;
using ToSic.Lib.Coding;
using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

internal static class ImageServiceTestAccessors
{
    public static IImageService ImgSvc(this TestBaseSxc parent)
        => parent.GetService<IImageService>();

    public static IImageService ImgSvc(this TestBaseSxcDb parent)
        => parent.GetService<IImageService>();

    public static IResizeSettings SettingsTac(this IImageService parent,
        object settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakResize, ITweakResize> tweak = default,
        object factor = default,
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string parameters = default,
        object recipe = default
    ) => parent.Settings(settings: settings, noParamOrder: noParamOrder, tweak: tweak,
        factor: factor, width: width, height: height,
        quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
        parameters: parameters, recipe: recipe);

    public static IResizeSettings SettingsTac(this TestBaseSxc parent,
        object settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakResize, ITweakResize> tweak = default,
        object factor = default,
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string parameters = default,
        object recipe = default
    ) => parent.ImgSvc().SettingsTac(settings: settings, noParamOrder: noParamOrder, tweak: tweak,
        factor: factor, width: width, height: height,
        quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
        parameters: parameters, recipe: recipe);

    public static IImageFormat GetFormatTac(this TestBaseSxcDb parent, string path) =>
        parent.ImgSvc().GetFormat(path);

    public static IList<IImageFormat> GetResizeFormatTac(this TestBaseSxcDb parent, string path) =>
        parent.GetFormatTac(path).ResizeFormats;
}