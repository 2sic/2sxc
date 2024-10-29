namespace ToSic.Sxc.Images.Internal;

[PrivateApi("Can and will change any time, don't use outside of 2sxc")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ImageConstants
{
    internal const int MaxSize = 3200;
    internal const int MaxQuality = 100;
    internal const string DontSetParam = "(none)";

    // https://imageresizing.net/docs/v4/reference
    public const string ModeCrop = "crop";
    public const string ModeMax = "max";
    public const string ModePad = "pad";
    public const string ModeCarve = "carve";
    public const string ModeStretch = "stretch";
    public const int IntIgnore = 0;

    /// <summary>
    /// In case a srcset is being generated with a '*' factor and we don't have a number, assume 1200.
    /// This is an ideal number, as it's quite big but not huge, and will usually be easily divisible by 2,3,4,6 etc.
    /// </summary>
    public const int FallbackWidthForSrcSet = 1200;

    public const int FallbackHeightForSrcSet = 0;

    internal static string FindKnownScaleOrNull(string scale)
    {
        // ReSharper disable RedundantCaseLabel
        // ReSharper disable StringLiteralTypo
        switch (scale?.ToLowerInvariant())
        {
            case "up":
            case "upscaleonly":
                return "upscaleonly";
            case "both":
                return "both";
            case "down":
            case "downscaleonly":
                return "downscaleonly";
            case null:
            default:
                return null;
        }
        // ReSharper restore RedundantCaseLabel
        // ReSharper restore StringLiteralTypo
    }


    // ----- ----- ----- Image Formats ----- ----- -----

    internal static string FindKnownFormatOrNull(string format)
    {
        switch (format?.ToLowerInvariant())
        {
            case Jpg:
            case "jpeg": return Jpg;
            case Png: return Png;
            case Gif: return Gif;
            case Webp: return Webp;
            default: return null;
        }
    }


    public const string Jpg = "jpg";
    public const string Gif = "gif";
    public const string Png = "png";
    public const string Svg = "svg";
    public const string Tif = "tif";
    public const string Webp = "webp";

    public static readonly Dictionary<string, ImageFormat> FileTypes = BuildFileTypes();

    /// <summary>
    /// Note: we're keeping our own list, because they are not many, and because the APIs in .net core/framework are different to find the mime types
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, ImageFormat> BuildFileTypes()
    {
        var webPInfo = new ImageFormat(Webp, "image/webp", true);
        var dic = new Dictionary<string, ImageFormat>(StringComparer.InvariantCultureIgnoreCase)
        {
            { Jpg, new ImageFormat(Jpg, "image/jpeg", true, new List<ImageFormat> { webPInfo }) },
            { Gif, new ImageFormat(Gif, "image/gif", true) },
            { Png, new ImageFormat(Png, "image/png", true, new List<ImageFormat> { webPInfo }) },
            { Svg, new ImageFormat(Svg, "image/svg+xml", false) },
            { Tif, new ImageFormat(Tif, "image/tiff", true) },
            { Webp, webPInfo }
        };
        dic["jpeg"] = dic[Jpg];
        dic["tiff"] = dic[Tif];
        return dic;
    }
}