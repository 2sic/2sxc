namespace ToSic.Sxc.Images.Internal;

/// <summary>
/// Describes everything to be known about an image format for resizing or generating source-tags.
/// </summary>
[PrivateApi("ATM no good reason to show this information")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IImageFormat
{
    /// <summary>
    /// The format name, like 'jpg' or 'png'
    /// </summary>
    string Format { get; }

    /// <summary>
    /// The Mime Type - if known
    /// </summary>
    string MimeType { get; }

    /// <summary>
    /// Information if this type can be resized
    /// </summary>
    bool CanResize { get; }

    /// <summary>
    /// Other formats this can be resized to, order by best to least good.
    /// 
    /// Usually used for creating source-tags in HTML
    /// </summary>
    IList<IImageFormat> ResizeFormats { get; }

}