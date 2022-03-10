using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// A object which contains everything to create HTML for responsive `img` tags with optimal `srcset` offering all the sizes you may need.
    ///
    /// You can simply add this object to the source, like `@image` to render the image - which is the same as `@image.ImgTag`
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IResponsiveImage: IHybridHtmlString
    {
        /// <summary>
        /// The `img` tag which would normally be added to the page automatically.
        /// You can also use the normal RazorBlade API and do things like `.Alt("description")` etc.
        /// See also the [RazorBlade Img docs](https://razor-blade.net/api/ToSic.Razor.Html5.Img.html)
        /// </summary>
        Img ImgTag { get; }

        /// <summary>
        /// The SrcSet in case you need to use it in your own custom img-tag
        /// </summary>
        string SrcSet { get; }

        /// <summary>
        /// The main url, commonly used for fallback `src` property
        /// </summary>
        string Url { get; }
    }
}