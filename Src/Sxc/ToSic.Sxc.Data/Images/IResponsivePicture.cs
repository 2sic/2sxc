using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Images;

/// <summary>
/// A object which contains everything to create HTML for responsive `picture` tags with optimal `srcset` offering all the sizes you may need
/// </summary>
/// <remarks>
/// History: Released 2sxc 13.10
/// </remarks>
[PublicApi]
public interface IResponsivePicture: IRawHtmlString, IResponsiveImage
{
    /// <summary>
    /// The `picture` tag with everything automatically included.
    /// See also the [RazorBlade Picture docs](https://razor-blade.net/api/ToSic.Razor.Html5.Picture.html)
    /// </summary>
    Picture Picture { get; }


    /// <summary>
    /// The `source` tags as they were auto-generated, in case you want to build the picture tag manually.
    /// Contains many `source` tags - see [RazorBlade Source docs](https://razor-blade.net/api/ToSic.Razor.Html5.Source.html)
    /// </summary>
    TagList Sources { get; }
}