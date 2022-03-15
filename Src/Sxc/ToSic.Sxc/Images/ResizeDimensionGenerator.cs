using System;
using ToSic.Eav.Logging;
using static ToSic.Sxc.Images.ImageConstants;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class ResizeDimensionGenerator: HasLog
    {
        public ResizeDimensionGenerator(): base("Img.ResDim") { }

        public bool Debug = false;


        public OneResize ResizeDimensions(ResizeSettings resizeSettings, OneResize optionalPrepared = null)
        {
            var factor = resizeSettings.Factor;
            if (DNearZero(factor)) factor = 1; // in this case we must still calculate, and should assume factor is exactly 1

            (int Width, int Height) initial = (
                optionalPrepared?.Width ?? resizeSettings.Width,
                optionalPrepared?.Height ?? resizeSettings.Height
            );

            (int Width, int Height) dim = (
                (resizeSettings.UseFactorMap ? FactorMapHelper.Find(resizeSettings.Advanced?.Factors, factor)?.Width : null) ?? (int)(factor * initial.Width),
                initial.Height
            );

            dim.Height = HeightFromAspectRatioOrFactor(dim, factor, resizeSettings.UseAspectRatio, resizeSettings.AspectRatio);

            dim = KeepInRangeProportional(dim);

            return new OneResize
            {
                Width = dim.Width,
                Height = dim.Height
            };
        }


        private int HeightFromAspectRatioOrFactor((int Width, int Height) dims, double factor, bool useAspectRatio, double aspectRatio)
        {
            var maybeLog = Debug ? Log : null;
            var wrapLog = maybeLog.SafeCall<int>();

            var hasAspectRatio = !DNearZero(aspectRatio);

            // 2. Figure out height, as we're resizing, we respect the aspect ratio, unless there is none or height shouldn't be set
            // Old: Width should only be calculated, if it wasn't explicitly provided (so only if coming from the settings)
            //var newW = dims.Width;

            // Height should only get Aspect Ratio if the Height wasn't specifically provided
            var newH = useAspectRatio && hasAspectRatio
                ? dims.Width / aspectRatio
                : dims.Height * factor;  // Note that often dims.H is 0, so this will still be 0

            //var final = ((int)dims.Width, (int)newH);
            return wrapLog($"H:{newH}", (int)newH);
        }


        internal (int W, int H) KeepInRangeProportional((int W, int H) original)
        {
            var maybeLog = Debug ? Log : null;
            var wrapLog = maybeLog.SafeCall<(int, int)>();

            // Simple case - it fits into the max-range
            if (original.W <= MaxSize && original.H <= MaxSize)
                return wrapLog("is already within bounds", original);

            // Harder - at least one doesn't fit - must figure out multiplier and adjust
            var correctionFactor = (float)Math.Max(original.W, original.H) / MaxSize;
            var newW = (int)Math.Min(original.W / correctionFactor, MaxSize);   // use Math.Min to avoid rounding errors leading to > 3200
            var newH = (int)Math.Min(original.H / correctionFactor, MaxSize);

            return wrapLog($"W:{newW}, H:{newH}", (newW, newH));
        }

        protected void IfDebugLogPair<T>(string prefix, (T W, T H) values)
        {
            if (Debug) Log.Add($"{prefix}: W:{values.W}, H:{values.H}");
        }
    }
}
