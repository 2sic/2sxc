using ToSic.Lib.Services;
using ToSic.Sxc.Internal.Plumbing;
using static ToSic.Sxc.Images.Internal.ImageConstants;
using static ToSic.Sxc.Images.RecipeVariant;
using static ToSic.Sxc.Internal.Plumbing.ParseObject;

namespace ToSic.Sxc.Images;

[PrivateApi("Internal stuff")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ResizeDimensionGenerator() : ServiceBase("Img.ResDim")
{
    internal bool Debug = false;
        

    /// <summary>
    /// Get the best matching dimension (width/height) based on what's specified
    /// </summary>
    private static int BestWidthOrHeightBasedOnSrcSet(int initial, int srcSetOverride, RecipeVariant partDef, int fallbackIfNoOriginal)
    {
        // SrcSet defined a value, use that
        if (srcSetOverride != 0) return srcSetOverride;

        // No need to recalculate anything, return original
        if (partDef.SizeType != SizePixelDensity && partDef.SizeType != SizeFactorOf) return initial;

        // If we're doing a factor-of, we always need an original value. If it's missing, use the fallback
        if (partDef.SizeType == SizeFactorOf && initial == 0) initial = fallbackIfNoOriginal;

        // Calculate the expected value based on Size=Scale-Factor * original
        return (int)(partDef.Size * initial);
    }


    internal OneResize ResizeDimensions(ResizeSettings settings, Recipe recipe, RecipeVariant partDef = null)
    {
        var factor = settings.FactorToUse;
        // if (DNearZero(factor)) factor = 1; // in this case we must still calculate, and should assume factor is exactly 1

        // First width - quite complicated
        var maybeWidth = FigureOutBestWidth(settings, recipe, partDef, factor);


        // Now height - might be incomplete, but ATM this isn't actually used much
        var probablyH = partDef == null
            ? settings.Height
            : BestWidthOrHeightBasedOnSrcSet(settings.Height, partDef.Height, partDef,
                FallbackHeightForSrcSet);

        (int Width, int Height) dim = (maybeWidth, probablyH);

        dim.Height = HeightFromAspectRatioOrFactor(dim, factor, settings.UseAspectRatio, settings.AspectRatio);

        dim = KeepInRangeProportional(dim);

        return new()
        {
            Width = dim.Width,
            Height = dim.Height
        };
    }

    private static int FigureOutBestWidth(ResizeSettings settings, Recipe recipe, RecipeVariant partDef, double factor)
    {
        // Priority 1: The value on the part definition. If it's non-zero, don't change the width by any other factor
        var width = partDef?.Width ?? 0;
        if (width != 0) return width;

        // Priority #2: The value from the factor map/recipe, which was selected based on this factor. 
        // It must be adjusted by part definition, as we may be looping through various sizes
        width = settings.UseFactorMap ? ParseObject.IntOrZeroAsNull(recipe?.Width) ?? 0 : 0;
        if (width != 0)
            return (int)(width * (partDef?.AdditionalFactor ?? 1));

        // Priority #3: If we have a Part-Definition, calculate the values now
        width = (int)(settings.Width * factor);

        if (partDef != null)
            width = BestWidthOrHeightBasedOnSrcSet(width, partDef.Width, partDef,
                FallbackWidthForSrcSet);

        return width;
    }


    private int HeightFromAspectRatioOrFactor((int Width, int Height) dims, double factor, bool useAspectRatio, double aspectRatio)
    {
        var maybeLog = Debug ? Log : null;
        var l = maybeLog.Fn<int>();

        var hasAspectRatio = !DNearZero(aspectRatio);

        // Height should only get Aspect Ratio if the Height wasn't specifically provided
        var newH = useAspectRatio && hasAspectRatio
            ? dims.Width / aspectRatio
            : dims.Height * factor;  // Note that often dims.H is 0, so this will still be 0

        return l.Return((int)newH, $"H:{newH}");
    }


    internal (int W, int H) KeepInRangeProportional((int W, int H) original)
    {
        var maybeLog = Debug ? Log : null;
        var l = maybeLog.Fn<(int, int)>();

        // Simple case - it fits into the max-range
        if (original.W <= MaxSize && original.H <= MaxSize)
            return l.Return(original, "is already within bounds");

        // Harder - at least one doesn't fit - must figure out multiplier and adjust
        var correctionFactor = (float)Math.Max(original.W, original.H) / MaxSize;
        var newW = (int)Math.Min(original.W / correctionFactor, MaxSize);   // use Math.Min to avoid rounding errors leading to > 3200
        var newH = (int)Math.Min(original.H / correctionFactor, MaxSize);

        return l.Return((newW, newH), $"W:{newW}, H:{newH}");
    }

}