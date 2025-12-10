using ToSic.Razor.Html5;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Images;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record ResponsiveImage: ResponsiveBase
{
    [PrivateApi("don't show")]
    internal ResponsiveImage(ImageService imgService, IPageService pageService, ResponsiveSpecs specs, ILog parentLog)
        : base(imgService, pageService, specs, parentLog, "Img")
    { }

    /// <summary>
    /// Same as base / initial implementation, but add srcset if available
    /// </summary>
    [field: AllowNull, MaybeNull]
    public override Img Img => field ??= GenerateImg();

    private Img GenerateImg()
    {
        var img = base.Img;
        if (!string.IsNullOrEmpty(SrcSet))
            img = img.Srcset(SrcSet);
        if (!string.IsNullOrEmpty(Sizes))
            img = img.Sizes(Sizes);
        return img;
    }

    /// <summary>
    /// Necessary so it also works with ToString() otherwise the record will show the JSON.
    /// </summary>
    public override string ToString() => ToHtmlString();
}