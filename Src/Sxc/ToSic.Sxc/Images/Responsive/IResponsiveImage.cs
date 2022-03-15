using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// # BETA
    /// A object which contains everything to create HTML for responsive `img` tags with optimal `srcset` offering all the sizes you may need.
    ///
    /// You can simply add this object to the source, like `@image` to render the image - which is the same as `@image.ImgTag`
    /// </summary>
    /// <remarks>
    /// History: **BETA** Released ca. 2sxc 13.10
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IResponsiveImage: IHybridHtmlString
    {
        /// <summary>
        /// The `img` tag which would normally be added to the page automatically.
        /// You can also use the normal RazorBlade API and do things like `.Alt("description")` etc.
        /// See also the [RazorBlade Img docs](https://razor-blade.net/api/ToSic.Razor.Html5.Img.html)
        /// </summary>
        Img Img { get; }

        /// <summary>
        /// The SrcSet in case you need to use it in your own custom img-tag.
        /// Note that it will be an empty string, if the image has no reason to have a srcset
        /// </summary>
        /// <remarks>
        /// Note that it's `Srcset` and _not_ `SrcSet` to be consistent with RazorBlade
        /// </remarks>
        string Srcset { get; }

        /// <summary>
        /// The main url, used for main `src` property
        /// </summary>
        string Url { get; }
    }
}