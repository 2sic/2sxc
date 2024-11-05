using ToSic.Lib.Helpers;
using ToSic.Razor.Html5;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Images.Internal;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ResponsiveImage: ResponsiveBase
{
    [PrivateApi("don't show")]
    internal ResponsiveImage(ImageService imgService, IPageService pageService, ResponsiveSpecs specs, ILog parentLog)
        : base(imgService, pageService, specs, parentLog, "Img")
    { }

    /// <summary>
    /// Same as base / initial implementation, but add srcset if available
    /// </summary>
    public override Img Img => _pictureImg.Get(() =>
    {
        var img = base.Img;
        if (!string.IsNullOrEmpty(SrcSet)) img = img.Srcset(SrcSet);
        if (!string.IsNullOrEmpty(Sizes)) img = img.Sizes(Sizes);
        return img;
    });
    private readonly GetOnce<Img> _pictureImg = new();

}