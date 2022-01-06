using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Images
{
    public class ImageFormat : IImageFormat
    {
        /// <inheritdoc />
        public string Format { get; }

        /// <inheritdoc />
        public string MimeType { get; }

        /// <inheritdoc />
        public bool CanResize { get; }

        public IList<ImageFormat> ResizeFormats { get; }

        public ImageFormat(string format, string mimeType, bool canResize, IEnumerable<ImageFormat> better = null)
        {
            Format = format;
            MimeType = mimeType;
            CanResize = canResize;
            ResizeFormats = canResize
                ? better?.Union(new []{this}).ToList() ?? new List<ImageFormat> { this }
                : new List<ImageFormat>();
        }
    }
}
