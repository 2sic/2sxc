using System.Collections.Generic;

namespace ToSic.Sxc.Images
{
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