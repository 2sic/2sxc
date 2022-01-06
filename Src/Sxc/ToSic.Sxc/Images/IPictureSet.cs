using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Images
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP")]
    public interface IPictureSet
    {
        /// <summary>
        /// The `img` tag which would normally be added to the page automatically. You can also use the normal RazorBlade API and do things like `.Alt("description")` etc.
        /// </summary>
        Img ImgTag { get; }

        /// <summary>
        /// The `picture` tag with everything automatically included.
        /// </summary>
        Picture PictureTag { get; }

        /// <summary>
        /// The `source` tags as they were auto-generated, in case you want to build the picture tag manually
        /// </summary>
        ITag SourceTags { get; }

        /// <summary>
        /// The main url, commonly used for fallback `src` property
        /// </summary>
        string Url { get; }
    }
}