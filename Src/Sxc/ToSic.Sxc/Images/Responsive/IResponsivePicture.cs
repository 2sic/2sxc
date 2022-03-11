using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// A object which contains everything to create HTML for responsive `picture` tags with optimal `srcset` offering all the sizes you may need
    /// </summary>
    /// <remarks>
    /// History: Released in 2sxc 13.03
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP")]
    public interface IResponsivePicture: IHybridHtmlString
    {
        /// <summary>
        /// The `img` tag which would normally be added to the page automatically.
        /// You can also use the normal RazorBlade API and do things like `.Alt("description")` etc.
        /// See also the [RazorBlade Img docs](https://razor-blade.net/api/ToSic.Razor.Html5.Img.html)
        /// </summary>
        Img Img { get; }

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

        /// <summary>
        /// The main url, commonly used for fallback `src` property on the image
        /// </summary>
        string Url { get; }
    }
}