using System;

namespace ToSic.Sxc.Web
{
    internal class ImgResizeLinker
    {
        internal const int MaxSize = 3200;
        internal const int MaxQuality = 100;
        
        internal static string KeepBestParam(object given, object setting)
        {
            if (given == null && setting == null) return null;
            var strGiven = RealStringOrNull(given);
            if (strGiven != null) return strGiven;
            var strSetting = RealStringOrNull(setting);
            return strSetting;
        }

        internal static string RealStringOrNull(object value)
        {
            if (value == null) return null;
            var strValue = value.ToString();
            return string.IsNullOrEmpty(strValue) ? null : strValue;
        }
        
        /// <summary>
        /// Check if an object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int? IntOrNull(object value)
        {
            if (value == null) return null;
            if (value is int intVal ) return intVal;

            var floatVal = FloatOrNull(value);
            if (floatVal == null) return null;
            
            var rounded = (int)Math.Round(floatVal.Value);
            if (rounded < 1) return null;
            return rounded;
        }

        internal static float? FloatOrNull(object value)
        {
            if (value == null) return null;
            if (value is float floatVal) return floatVal;
            if (value is double dVal) return (float)dVal;

            var strValue = RealStringOrNull(value);
            if (strValue == null) return null;
            if (!double.TryParse(strValue, out var doubleValue)) return null;
            return (float)doubleValue;
        }

        internal static Tuple<int, int> Rescale(int width, int height, float factor, float aspectRatio, bool scaleW, bool scaleH)
        {
            // Check if we have nothing to rescale
            if (width == 0 && height == 0) return new Tuple<int, int>(width, height);
            if (factor == 0f || Math.Abs(factor - 1) < 0.01) return new Tuple<int, int>(width, height); // No factor - 0 or 1
            if (!scaleW && !scaleH) return new Tuple<int, int>(width, height);
            
            // Figure out height/width, as we're resizing, we respect the aspect ratio, unless there is none or height shouldn't be set
            var newW = !scaleW ? width : width * factor;
            var newH = (!scaleH || height == 0)
                ? height
                : (aspectRatio != 0 && Math.Abs(aspectRatio - 1) < 0.01)
                    ? height * factor
                    : newW / aspectRatio;

            // new - don't check here
            return new Tuple<int, int>((int)newW, (int)newH);
        }

        internal static Tuple<int, int> KeepInRangeProportional(Tuple<int, int> original)
        {
            // Simple case - it fits into the max-range
            if (original.Item1 <= MaxSize && original.Item2 <= MaxSize) return original;
            
            // Harder - at least one doesn't fit - must figure out multiplier and adjust
            var correctionFactor = (float)Math.Max(original.Item1, original.Item2) / MaxSize;
            var newW = (int)Math.Min(original.Item1 / correctionFactor, MaxSize);   // use Math.Min to avoid rounding errors leading to > 3200
            var newH = (int)Math.Min(original.Item2 / correctionFactor, MaxSize);
            return new Tuple<int, int>(newW, newH);

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
