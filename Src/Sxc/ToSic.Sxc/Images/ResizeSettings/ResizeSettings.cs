using System.Collections.Specialized;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Images.Internal;
using static ToSic.Sxc.Images.Internal.ImageConstants;
using static ToSic.Sxc.Internal.Plumbing.ParseObject;

namespace ToSic.Sxc.Images;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal record ResizeSettings : IResizeSettings, IResizeSettingsInternal
{
    /// <summary>
    /// Name of the settings used initially
    /// </summary>
    public string BasedOn { get; }

    public int Width { get; init; } = IntIgnore;
    public int Height { get; init; } = IntIgnore;
    public int Quality { get; set; } = IntIgnore;
    public string ResizeMode { get; set; }
    public string ScaleMode { get; set; }
    public string Format { get; init; }
    public double Factor { get; init; } = 1;
    public double AspectRatio { get; init; }
    public NameValueCollection Parameters { get; set; }


    public bool UseFactorMap { get; set; } = true;
    public bool UseAspectRatio { get; set; } = true;

    public AdvancedSettings Advanced { get; set; }

    /// <summary>
    /// Constructor to create new
    /// </summary>
    public ResizeSettings(int? width, int? height, int fallbackWidth, int fallbackHeight, double aspectRatio, double factor, string format, string basedOn)
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
        BasedOn = basedOn;
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

        BasedOn = original.BasedOn;

        // workaround, as it's not part of the interface ATM
        if (original is ResizeSettings typed)
        {
            UseAspectRatio = typed.UseAspectRatio;
            UseFactorMap = typed.UseFactorMap;
        }

        Advanced = advanced ?? (original as IResizeSettingsInternal)?.Advanced;
    }

    /// <summary>
    /// Should the factor be used? only if it's clearly away from zero.
    /// </summary>
    private bool FactorUsed => !DNearZero(Factor) && !DNearZero(Factor - 1); // so ~0 and ~1 are not used

    /// <summary>
    /// The factor to use, which is either the factor, or 1 if it's not used
    /// </summary>
    internal double FactorToUse => FactorUsed ? Factor : 1;

    internal string ToHtmlInfo(ImageDecorator decoOrNull)
    {
        const string notSet = "default/not set";
        const double floatTolerance = 0.01;

        // Special message for height, in case it has a decorator
        // which overrules the default height
        var height = $"{(Height != IntIgnore ? Height : notSet)}";
        var resize = ResizeMode ?? notSet;
        var scale = ScaleMode ?? notSet;
        var cropCenter = ImageDecorator.DefaultCropCenter;
        var aspectRatio = $"{(Math.Abs(AspectRatio - IntIgnore) > floatTolerance ? AspectRatio : notSet)}";
        

        var noCrop = decoOrNull?.CropBehavior == ImageDecorator.NoCrop;
        if (noCrop)
        {
            height = $"<s>{height}</s> - no-crop = use image as is";
            resize = $"<s>{resize}</s> - {ImageDecorator.NoCrop}";
            aspectRatio = $"<s>{aspectRatio}</s> - no-crop";
            cropCenter = $"<s>{cropCenter}</s> - no-crop";
        }
        else
        {
            if (!UseAspectRatio)
                aspectRatio = $"<s>{aspectRatio}</s> - not used, height is set";
            if (decoOrNull?.CropBehavior == ImageDecorator.ToCrop)
                cropCenter = decoOrNull.CropToNiceName;
        }

        var basedOn = BasedOn.HasValue() ? $"<strong>Settings.Images.{BasedOn}</strong>" : "<em>no presets</em>";
        var hasSpecialSettings = decoOrNull != null ? "✅" : "no";

        var result = $@"<strong>🖼️ Resize Specs</strong>

Based On: {basedOn}
Custom Image Settings: {hasSpecialSettings}
Width: {(Width != IntIgnore ? Width : notSet)}
Height: {height}
Quality: {(Quality != IntIgnore ? Quality : notSet)}
ResizeMode: {resize}
ScaleMode: {scale}
Format: {Format ?? notSet}
Factor: {(FactorUsed ? Factor.ToString("0.####") : notSet)}
AspectRatio: {aspectRatio}
Url Parameters: {Parameters}
Crop Center: {cropCenter}

<em>This is the primary size. Other responsive sizes derive from this.</em>";
        
        return result.Replace(", ", "\n").Replace("\n", "<br>\n");
    }
}