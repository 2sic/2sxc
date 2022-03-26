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
        /// Determines if the image should be shown entirely.
        /// This usually means that the image is a logo or something, so cropping was not an option.
        /// This also usually means that the aspect ratio / height may be different than expected
        /// </summary>
        bool ShowAll { get; }

        /// <summary>
        /// The image height, if it should be set at all. Will be null otherwise. 
        /// </summary>
        string Height { get; }

        /// <summary>
        /// The image width, if it should be set at all. Will be null otherwise. 
        /// </summary>
        string Width { get; }

        /// <summary>
        /// The SrcSet in case you need to use it in your own custom img-tag.
        /// Note that it will be an empty string, if the image has no reason to have a srcset
        /// </summary>
        string SrcSet { get; }

        /// <summary>
        /// The main url, used for main `src` property
        /// </summary>
        string Url { get; }
    }
}