using System.Collections.Specialized;
using static ToSic.Sxc.Images.Internal.ImageConstants;

namespace ToSic.Sxc.Images;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ResizeSettings : IResizeSettings, IResizeSettingsInternal
{
    public int Width { get; } = IntIgnore;
    public int Height { get; } = IntIgnore;
    public int Quality { get; set; } = IntIgnore;
    public string ResizeMode { get; set; }
    public string ScaleMode { get; set; }
    public string Format { get; }
    public double Factor { get; } = 1;
    public double AspectRatio { get; }
    public NameValueCollection Parameters { get; set; }


    public bool UseFactorMap { get; set; } = true;
    public bool UseAspectRatio { get; set; } = true;

    public AdvancedSettings Advanced { get; set; }

    /// <summary>
    /// Constructor to create new
    /// </summary>
    public ResizeSettings(int? width, int? height, int fallbackWidth, int fallbackHeight, double aspectRatio, double factor, string format)
    {
        Width = width ?? fallbackWidth;
        Height = height ?? fallbackHeight;
        // If the width was given by the parameters, then don't use FactorMap
        UseFactorMap = width == null;
        // If the height was supplied by parameters, don't use aspect ratio
        UseAspectRatio = height == null;

        AspectRatio = aspectRatio;
        Factor = factor;
        Format = format;
    }

    /// <summary>
    /// Constructor to copy
    /// </summary>
    public ResizeSettings(
        IResizeSettings original,
        NoParamOrder noParamOrder = default,
        int? width = null,
        int? height = null,
        double? aspectRatio = null,
        double? factor = null,
        int? quality = null,
        string format = null,
        string resizeMode = null,
        string scaleMode = null,
        NameValueCollection parameters = null,
        AdvancedSettings advanced = null
    )
    {
        if (original == null) throw new ArgumentNullException(nameof(original));
        Width = width ?? original.Width;
        Height = height ?? original.Height;
        Quality = quality ?? original.Quality;
        ResizeMode = resizeMode ?? original.ResizeMode;
        ScaleMode = scaleMode ?? original.ScaleMode;
        Format = format ?? original.Format;
        Factor = factor ?? original.Factor;
        Parameters = parameters ?? original.Parameters;
        AspectRatio = aspectRatio ?? original.AspectRatio;

        // workaround, as it's not part of the interface ATM
        if (original is ResizeSettings typed)
        {
            UseAspectRatio = typed.UseAspectRatio;
            UseFactorMap = typed.UseFactorMap;
        }

        Advanced = advanced ?? (original as IResizeSettingsInternal)?.Advanced;
    }

}