using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Images.Sys;

[PrivateApi("Hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ImageFormat : IImageFormat
{
    /// <inheritdoc />
    public string Format { get; }

    /// <inheritdoc />
    public string MimeType { get; }

    /// <inheritdoc />
    public bool CanResize { get; }

    public IList<IImageFormat> ResizeFormats { get; }

    public ImageFormat(string format, string mimeType, bool canResize, IEnumerable<IImageFormat>? better = null)
    {
        Format = format;
        MimeType = mimeType;
        CanResize = canResize;
        ResizeFormats = canResize
            ? better?.Union([this]).ToList() ?? [this]
            : [];
    }

    public ImageFormat(IImageFormat original, bool preserveSizes)
    {
        Format = original.Format;
        MimeType = original.MimeType;
        CanResize = original.CanResize;
        ResizeFormats = preserveSizes ? original.ResizeFormats : new List<IImageFormat>();
    }
}