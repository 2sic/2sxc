using ToSic.Sxc.Adam;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cms.Sys;

internal class HtmlImgToPictureHelper(CmsServiceImageExtractor imageExtractor)
    : ServiceWithContext("Cms.StrWys", connect: [imageExtractor])
{
    [field: AllowNull, MaybeNull]
    internal IImageService ImageService => field
        ??= ExCtx.GetService<IImageService>(reuse: true);


    public IResponsivePicture ConvertImgToPicture(string originalImgTag, IFolder folder, object? defaultImageSettings)
    {
        var imgProps = imageExtractor.ExtractImageProperties(originalImgTag, folder);

        // if we have a real file, pre-get the inner parameters as we would want to use it for resize-settings
        var preparedImgParams = imgProps.File.NullOrGetWith(ResponsiveSpecsOfTarget.ExtractSpecs);

        // if the file itself specifies a resize settings, use it, otherwise use the default settings
        var imgSettings = preparedImgParams?.ImgDecoratorOrNull?.ResizeSettings ?? defaultImageSettings;

        // In most cases use the preparedImgParams, but if it's null, use the src attribute
        var target = (object?)preparedImgParams ?? imgProps.Src;

        // use the IImageService to create Picture tags for it
        var picture = ImageService.Picture(link: target, settings: imgSettings, factor: imgProps.Factor, width: imgProps.Width,
            imgAlt: imgProps.ImgAlt, imgClass: imgProps.ImgClasses, imgAttributes: imgProps.OtherAttributes,
            pictureClass: imgProps.PicClasses);
        return picture;
    }

}