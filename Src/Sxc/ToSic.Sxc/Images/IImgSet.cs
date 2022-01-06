using ToSic.Eav.Documentation;
using ToSic.Razor.Html5;

namespace ToSic.Sxc.Images
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IImgSet
    {
        /// <summary>
        /// A `img` tag to either add directly to the page, or you can also use the normal RazorBlade API and do things like `.Alt("description")` etc.
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