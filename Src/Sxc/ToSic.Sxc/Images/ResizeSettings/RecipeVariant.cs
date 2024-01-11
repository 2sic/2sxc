using System.Globalization;

namespace ToSic.Sxc.Images;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class RecipeVariant
{
    public const char SizeDefault = 'd';
    public const char SizeWidth = 'w';
    public const char SizePixelDensity = 'x';
    public const char SizeFactorOf = '*';
    public static readonly char[] SizeTypes = [SizeWidth, SizePixelDensity, SizeDefault, SizeFactorOf];

    /// <summary>
    /// The size - usually 1000 or something in case of 'w', and 1.5 or something in case of 'x'
    /// </summary>
    public double Size;


    public double AdditionalFactor = 1;

    /// <summary>
    /// Type of size - width or pixel density
    /// </summary>
    public char SizeType = SizeDefault;

    /// <summary>
    /// Image width if specified, in pixels
    /// </summary>
    public int Width;

    /// <summary>
    /// Image height if specified in pixels
    /// </summary>
    public int Height;

    public string SrcSetSuffix(int finalWidth)
    {
        var srcSetSize = Size;
        var srcSetSizeTypeCode = SizeType;
        if (srcSetSizeTypeCode == SizeFactorOf)
        {
            srcSetSize = finalWidth;
            srcSetSizeTypeCode = SizeWidth;
        }

        var suffix = $" {srcSetSize.ToString(CultureInfo.InvariantCulture)}{srcSetSizeTypeCode}";
        return suffix;
    }

}