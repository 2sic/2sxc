using System;

namespace ToSic.Sxc.Web
{
    internal class ImgResizeLinker
    {
        internal const int MaxSize = 3200;
        internal const int MaxQuality = 100;
        
        internal static int ImgKeepInRange(string value, int max, float factor = 1)
        {
            if (value == null) return 0;
            if (!double.TryParse(value, out var asDouble)) return 0;
            var rounded = (int)Math.Round(asDouble);
            var multiplied = rounded * factor;
            return multiplied > max ? max : (int)multiplied;
        }

        internal static string KeepBestParam(object given, object setting)
        {
            if (given == null && setting == null) return null;
            var strGiven = ToNullOrString(given);
            if (strGiven != null) return strGiven;
            var strSetting = ToNullOrString(setting);
            return strSetting;
        }

        internal static string ToNullOrString(object value)
        {
            if (value == null) return null;
            var strValue = value.ToString();
            return string.IsNullOrEmpty(strValue) ? null : strValue;
        }

        internal static Tuple<int, int> Rescale(int width, int height, string factorString, string aspectRatioString)
        {
            // Check if we have nothing to rescale
            if (width == 0 && height == 0) return new Tuple<int, int>(width, height);
            
            // Factor checks
            float factor = 1;
            if (factorString != null)
                if (!float.TryParse(factorString, out factor))
                    return new Tuple<int, int>(width, height);
            
            // Aspect Ratio check
            float aspectRatio = 1;
            if (aspectRatioString != null)
                float.TryParse(aspectRatioString, out aspectRatio);

            // No factor - 0 or 1
            if (Math.Abs(factor - 1) < 0.01 || factor == 0f) return new Tuple<int, int>(width, height);
            
            // Figure out height/width, as we're resizing, we respect the aspect ratio, unless there is none or height shouldn't be set
            var newW = width * factor;
            var newH = height == 0
                ? 0
                : Math.Abs(aspectRatio - 1) < 0.01
                    ? height * factor
                    : newW / aspectRatio;

            // Simple case - it fits into the max-range
            if (newH <= MaxSize && newW <= MaxSize) return new Tuple<int, int>((int)newW, (int)newH);
            
            // Harder - at least one doesn't fit - must figure out multiplier and adjust
            var correctionFactor = Math.Max(newW, newH) / MaxSize;
            newW = Math.Min(newW/ correctionFactor, MaxSize);   // use Math.Min to avoid rounding errors leading to > 3200
            newH = Math.Min(newH / correctionFactor, MaxSize);
            return new Tuple<int, int>((int)newW, (int)newH);
        }

        internal static string CorrectScales(string scale)
        {
            // ReSharper disable RedundantCaseLabel
            switch (scale?.ToLowerInvariant())
            {
                case "up":
                case "upscaleonly":
                    return "upscaleonly";
                case "both":
                    return "both";
                case null:
                case "down":
                case "downscaleonly":
                default: 
                    return null;
            }
            // ReSharper restore RedundantCaseLabel
        }

        internal static string CorrectFormats(string format)
        {
            switch (format?.ToLowerInvariant()) 
            {
                case "jpg":
                case "jpeg": return "jpg";
                case "png": return "png";
                case "gif": return "gif";
                default: return null;
            }
        }
    }
}
