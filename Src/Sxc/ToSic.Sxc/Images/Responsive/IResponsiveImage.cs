using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Images;

/// <summary>
/// A object which contains everything to create HTML for responsive `img` tags with optimal `srcset` offering all the sizes you may need.
///
/// You can simply add this object to the source, like `@image` to render the image - which is the same as `@image.ImgTag`
/// </summary>
/// <remarks>
/// History: Released 2sxc 13.10
/// </remarks>
[PublicApi]
public interface IResponsiveImage: IRawHtmlString
{
    /// <summary>
    /// An Alt-description on the image which is retrieved from (in order of priority):
    /// 
    /// 1. the Razor code creating this object using the parameter `imgAlt`
    /// 2. or from image metadata - see <see cref="Description"/>
    /// 3. or from the Razor code using the parameter `imgAltFallback` _new v15_
    /// </summary>
    string Alt { get; }

    /// <summary>
    /// The Class of the image. Usually created from these sources
    /// - The initial call creating this image tag
    /// - Resize-Settings which may add classes
    /// - Rule which determines if the image should crop or not, which may add a class
    /// </summary>
    string Class { get; }

    /// <summary>
    /// Image description from the image Metadata.
    /// See also <see cref="Alt"/>.
    /// </summary>
    /// <returns>
    /// * `null` if no metadata exists
    /// * `""` empty string if metadata exists but no description was given
    /// * a string containing the added description
    /// </returns>
    /// <remarks>Added in v15</remarks>
    string Description { get; }

    /// <summary>
    /// Extended description, typically used in galleries.
    /// </summary>
    /// <remarks>Added in v16.04</remarks>
    string DescriptionExtended { get; }

    /// <summary>
    /// The `img` tag which would normally be added to the page automatically.
    /// You can also use the normal RazorBlade API and do things like `.Alt("description")` etc.
    /// See also the [RazorBlade Img docs](https://razor-blade.net/api/ToSic.Razor.Html5.Img.html)
    /// </summary>
    Img Img { get; }

    /// <summary>
    /// The outermost tag - name not yet final
    /// </summary>
    [PrivateApi("Name not yet final")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IHtmlTag Tag { get; }

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
    /// Note that it will be null if the image has no reason to have a srcset.
    ///
    /// It will only be used for normal `img` tags, but not for `img` tags inside `picture` tags.
    /// </summary>
    string SrcSet { get; }

    /// <summary>
    /// The sizes in case you need it in your custom img-tag.
    /// It will only be used for normal `img` tags, but not for `img` tags inside `picture` tags.
    /// </summary>
    string Sizes { get; }

    /// <summary>
    /// Get the toolbar to show it on another tag (typically a `figure` around the `picture`)
    /// or set another toolbar instead.
    /// </summary>
    /// <remarks>Added in v16.04</remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP 16.04, may still change")]
    IToolbarBuilder Toolbar();

    /// <summary>
    /// The main url, used for main `src` property
    /// </summary>
    /// <remarks>Added in v13.11</remarks>
    string Src { get; }
}