using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Images;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class ImgResizeLinker: ImageLinkerBase // HasLog
    {
        internal const int MaxSize = 3200;
        internal const int MaxQuality = 100;

        //public bool Debug = false;

        public ImgResizeLinker() : base($"{Constants.SxcLogName}.ImgRes")
        {
        }


        internal override Tuple<int, int> FigureOutBestWidthAndHeight(object width, object height, object factor, object aspectRatio, ICanGetNameNotFinal getSettings)
        {
            // Try to pre-process parameters and prefer them
            var parms = new Tuple<int?, int?>(IntOrNull(width), IntOrNull(height));
            IfDebugLogPair("Params", parms);

            // Pre-Clean the values - all as strings
            var set = new Tuple<dynamic, dynamic>(getSettings?.Get("Width"), getSettings?.Get("Height"));
            if(getSettings!=null) IfDebugLogPair("Settings", set);

            var safe = new Tuple<int, int>(parms.Item1 ?? IntOrNull(set.Item1) ?? 0, parms.Item2 ?? IntOrNull(set.Item2) ?? 0);
            IfDebugLogPair("Safe", safe);


            var factorFinal = FloatOrNull(factor) ?? 0;
            var arFinal = FloatOrNull(aspectRatio)
                          ?? FloatOrNull(getSettings?.Get("AspectRatio")) ?? 0;
            if (Debug) Log.Add($"Resize Factor: {factorFinal}, Aspect Ratio: {arFinal}");

            // if either param h/w was null, then do a rescaling on the param which comes from the settings
            // But ignore the other one!
            var rescale = factorFinal != 0 && (parms.Item1 == null || parms.Item2 == null);
            Tuple<int, int> resizedNew = rescale
                ? Rescale(safe/*.Item1, safe.Item2*/, factorFinal, arFinal, parms.Item1 == null, parms.Item2 == null)
                : safe;
            IfDebugLogPair("Rescale", resizedNew);

            resizedNew = KeepInRangeProportional(resizedNew);
            return resizedNew;
        }

        private void IfDebugLogPair<T>(string prefix, Tuple<T, T> values)
        {
            if (!Debug) return;
            Log.Add($"{prefix}: W:{values.Item1}, H:{values.Item2}");
        }





        internal override string KeepBestParam(object given, object setting)
        {
            if (given == null && setting == null) return null;
            var strGiven = RealStringOrNull(given);
            if (strGiven != null) return strGiven;
            var strSetting = RealStringOrNull(setting);
            return strSetting;
        }

        internal override string RealStringOrNull(object value)
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
        internal override int? IntOrNull(object value)
        {
            if (value == null) return null;
            if (value is int intVal ) return intVal;

            var floatVal = FloatOrNull(value);
            if (floatVal == null) return null;
            
            var rounded = (int)Math.Round(floatVal.Value);
            if (rounded < 1) return null;
            return rounded;
        }

        internal float? FloatOrNull(object value)
        {
            if (value == null) return null;
            if (value is float floatVal) return floatVal;
            if (value is double dVal) return (float)dVal;

            var strValue = RealStringOrNull(value);
            if (strValue == null) return null;
            if (!double.TryParse(strValue, out var doubleValue)) return null;
            return (float)doubleValue;
        }

        internal Tuple<int, int> Rescale(Tuple<int, int> dims, /*int width, int height,*/ float factor, float aspectRatio, bool scaleW, bool scaleH)
        {
            var maybeLog = Debug ? Log : null;
            var wrapLog = maybeLog.SafeCall<Tuple<int, int>>();

            // Check if we have nothing to rescale
            string msgWhyNoRescale = null;
            if (dims.Item1 == 0 && dims.Item2 == 0) msgWhyNoRescale = "w/h == 0";
            if (factor == 0f || Math.Abs(factor - 1) < 0.01) msgWhyNoRescale = "Factor is 0 or 1";
            if (!scaleW && !scaleH) msgWhyNoRescale = "h/w shouldn't be scaled";
            if (msgWhyNoRescale != null)
                return wrapLog(msgWhyNoRescale + ", no changes", dims);// new Tuple<int, int>(width, height));
            
            // Figure out height/width, as we're resizing, we respect the aspect ratio, unless there is none or height shouldn't be set
            var newW = !scaleW ? dims.Item1 : dims.Item1 * factor;
            float newH = dims.Item2;
            var doScaleH = scaleH && dims.Item2 != 0;
            var useAspectRatio = aspectRatio == 0 || !(Math.Abs(aspectRatio - 1) < 0.01);

            maybeLog.SafeAdd($"ScaleW: {scaleW}, ScaleH: {scaleH}, Really-ScaleH:{doScaleH}, Use Aspect Ratio:{useAspectRatio}");

            if (doScaleH) newH = useAspectRatio ? newW / aspectRatio : dims.Item2 * factor;

            // new - don't check here
            var intW = (int)newW;
            var intH = (int)newH;
            return wrapLog($"W:{intW}, H:{intH}", new Tuple<int, int>(intW, intH));
        }

        internal Tuple<int, int> KeepInRangeProportional(Tuple<int, int> original)
        {
            var maybeLog = Debug ? Log : null;
            var wrapLog = maybeLog.SafeCall<Tuple<int, int>>();

            // Simple case - it fits into the max-range
            if (original.Item1 <= MaxSize && original.Item2 <= MaxSize) 
                return wrapLog("is already within bounds", original);
            
            // Harder - at least one doesn't fit - must figure out multiplier and adjust
            var correctionFactor = (float)Math.Max(original.Item1, original.Item2) / MaxSize;
            var newW = (int)Math.Min(original.Item1 / correctionFactor, MaxSize);   // use Math.Min to avoid rounding errors leading to > 3200
            var newH = (int)Math.Min(original.Item2 / correctionFactor, MaxSize);

            return wrapLog($"W:{newW}, H:{newH}", new Tuple<int, int>(newW, newH));
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
                case "down":
                case "downscaleonly":
                    return "downscaleonly";
                case null:
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
