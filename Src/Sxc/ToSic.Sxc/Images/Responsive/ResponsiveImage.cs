using ToSic.Lib.Helpers;
using ToSic.Razor.Html5;

namespace ToSic.Sxc.Images;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ResponsiveImage: ResponsiveBase
{
    [PrivateApi("don't show")]
    internal ResponsiveImage(ImageService imgService, ResponsiveParams callParams, ILog parentLog) : base(imgService, callParams, parentLog, "Img")
    {
    }

    /// <summary>
    /// Same as base / initial implementation, but add srcset if available
    /// </summary>
    public override Img Img => _img2.Get(() =>
    {
        var img = base.Img;
        if (!string.IsNullOrEmpty(SrcSet)) img = img.Srcset(SrcSet);
        if (!string.IsNullOrEmpty(Sizes)) img = img.Sizes(Sizes);
        return img;
    });
    private readonly GetOnce<Img> _img2 = new();

}