using System.IO;
using ToSic.Sxc.Images.Internal;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;

namespace ToSic.Sxc.Images;

partial class ImageService
{
    /// <inheritdoc />
    public IImageFormat GetFormat(string path)
    {
        // 1. check extension makes sense / lower case
        if (string.IsNullOrWhiteSpace(path)) return new ImageFormat("", "", false);
        path = path.Split('?')[0];
        var extension = path.ToLowerInvariant();
        if (extension.Contains(".")) extension = Path.GetExtension(extension).Trim('.');

        // 2. See if we know of this - if yes, return - but strip sub-formats if the feature is disabled
        if (ImageConstants.FileTypes.TryGetValue(extension, out var result))
            return Features.IsEnabled(ImageServiceMultiFormat.NameId)
                ? result
                : new(result, false);

        // 3. Otherwise just return an object without known mime type
        return new ImageFormat(extension, "", false);

        // 4. Future / maybe: Otherwise check system for mime type and try to build a recommendation
    }
        
}