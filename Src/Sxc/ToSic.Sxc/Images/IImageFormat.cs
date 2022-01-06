using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// Describes everything to be known about an image format for resizing or generating source-tags.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
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
        IList<ImageFormat> ResizeFormats { get; }

    }
}