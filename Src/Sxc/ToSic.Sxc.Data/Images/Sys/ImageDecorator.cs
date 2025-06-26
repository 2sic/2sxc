﻿using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Images.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ImageDecorator(IEntity entity, string?[] languageCodes)
    : EntityBasedType(entity, languageCodes), IImageDecorator
{
    #region Constants and Type Names

    public static string TypeNameId = "cb27a0f2-f921-48d0-a3bc-37c0e77b1d0c";
    public static string NiceTypeName = "ImageDecorator";

    public const string NoCrop = "none";
    public const string ToCrop = "to";
    public const string DefaultCropCenter = "Middle Center";

    private static readonly Dictionary<string, string> ToCropNiceNames = new()
    {
        { "tl", "Top Left" },
        { "tc", "Top Center" },
        { "tr", "Top Right" },
        { "ml", "Middle Left" },
        { "mc", "Middle Center" },
        { "mr", "Middle Right" },
        { "bl", "Bottom Left" },
        { "bc", "Bottom Center" },
        { "br", "Bottom Right" }
    };

    /// <summary>
    /// Parameter to give the UI when it should show a warning for a global file
    /// </summary>
    public const string ShowWarningGlobalFile = "showWarningGlobalFile";

    #endregion


    public static ImageDecorator? GetOrNull(IHasMetadata? source, string?[] dimensions)
    {
        var decItem = source?.Metadata?.FirstOrDefaultOfType(TypeNameId);
        return decItem != null
            ? new ImageDecorator(decItem, dimensions)
            : null;
    }

    #region Cropping

    public string CropBehavior => GetThis("");

    public string CropTo => GetThis("");

    internal string CropToNiceName => ToCropNiceNames.TryGetValue(CropTo, out var name) ? name : DefaultCropCenter;

    #endregion

    #region Descriptions / Titles

    public string Description => GetThis("");

    /// <summary>
    /// Disable falling back to the default/alternate title
    /// </summary>
    public bool SkipFallbackTitle => GetThis(false);

    /// <summary>
    /// Detailed description of an image
    /// </summary>
    public string? DescriptionExtended => GetThis<string>(null);

    #endregion

    /// <summary>
    /// EXPERIMENTAL V18 not implemented yet
    /// </summary>
    public string ResizeSettings => GetThis("");

    #region Lightbox - new v18

    /// <summary>
    /// Important: Triple-State: can be null even if the entity exists, 
    /// </summary>
    public bool? LightboxIsEnabled => GetThis<bool?>(null);

    public string LightboxGroup => GetThis("");

    #endregion


    internal (string? Param, string? Value) GetAnchorOrNull()
    {
        var b = CropBehavior;
        if (b != ToCrop)
            return (null, null);
        
        var direction = CropTo;
        if (string.IsNullOrWhiteSpace(direction))
            return (null, null);

        var dirLong = ResolveCompass(direction);
        if (string.IsNullOrWhiteSpace(dirLong))
            return (null, null);

        return ("anchor", dirLong);
    }

    #region Private Gets


    private string? ResolveCompass(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 2)
            return null;
        return GetRow(code[0]) + GetCol(code[1]);
    }

    private static string? GetRow(char code)
    {
        switch (code)
        {
            case 't': return "top";
            case 'm': return "middle";
            case 'b': return "bottom";
            default: return null;
        }
    }
    private static string? GetCol(char code)
    {
        switch (code)
        {
            case 'c': return "center";
            case 'l': return "left";
            case 'r': return "right";
            default: return null;
        }
    }

    #endregion
}