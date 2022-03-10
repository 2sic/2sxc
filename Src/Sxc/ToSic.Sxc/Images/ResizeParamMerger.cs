using System;
using ToSic.Eav.Logging;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Images.ImageConstants;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// This merges predefined settings with custom specified parameters to create a stable resize-Parameters object for further use
    /// </summary>
    internal class ResizeParamMerger: HasLog // <ResizeParamMerger>
    {
        private const string ResizeModeField = "ResizeMode";
        private const string ScaleModeField = "ScaleMode";
        private const string QualityField = "Quality";
        private const string WidthField = "Width";
        private const string HeightField = "Height";
        private const string AspectRatioField = "AspectRatio";
        private const string SrcSetField = "SrcSet";

        public ResizeParamMerger() : base(Constants.SxcLogName + ".ImgRPM") { }

        public bool Debug = false;




        internal IResizeSettings BuildResizeSettings(
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string parameters = null,
            object srcset = null
            )
        {
            var wrapLog = (Debug ? Log : null).SafeCall<string>();
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(BuildResizeSettings)}", $"{nameof(settings)},{nameof(factor)},{nameof(width)}, ...");

            // check common mistakes
            if (aspectRatio != null && height != null)
            {
                wrapLog("error", null);
                const string messageOnlyOneOrNone = "only one or none of these should be provided, other can be zero";
                throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);
            }

            // Check if the settings is the expected type or null/other type
            var getSettings = settings as ICanGetNameNotFinal;
            if (Debug) Log.Add($"Has Settings:{getSettings != null}");

            var resizeParams = FigureOutBestWidthAndHeight(width, height, factor, aspectRatio, getSettings);

            // Add parameters if known
            if (!string.IsNullOrWhiteSpace(parameters))
                resizeParams.Parameters = UrlHelpers.ParseQueryString(parameters);

            resizeParams.Format = FindKnownFormatOrNull(RealStringOrNull(format));

            // Aspects which aren't affected by scale
            var qParamDouble = DoubleOrNull(quality);
            if (qParamDouble.HasValue)
                qParamDouble = DNearZero(qParamDouble.Value)  // ignore if basically 0
                    ? null
                    : qParamDouble.Value > 1
                        ? qParamDouble
                        : qParamDouble * 100;
            var qParamInt = (int?)qParamDouble;
            resizeParams.Quality = qParamInt ?? IntOrZeroAsNull(getSettings?.Get(QualityField)) ?? 0;
            resizeParams.ResizeMode = KeepBestString(resizeMode, getSettings?.Get(ResizeModeField));
            resizeParams.ScaleMode = FindKnownScaleOrNull(KeepBestString(scaleMode, getSettings?.Get(ScaleModeField)));

            resizeParams.SrcSet = srcset is string srcSetString
                ? srcSetString
                : srcset is bool srcSetBool && srcSetBool ? getSettings?.Get(SrcSetField) : null;
            
            return resizeParams;
        }

        internal ResizeSettings FigureOutBestWidthAndHeight(object width, object height, object factor, object aspectRatio, ICanGetNameNotFinal settingsOrNull)
        {
            // Try to pre-process parameters and prefer them
            // The manually provided values must remember Zeros because they deactivate presets
            (int? W, int? H) parms = (IntOrNull(width), IntOrNull(height));
            IfDebugLogPair("Params", parms);

            // Pre-Clean the values - all as strings
            (dynamic W, dynamic H) set = (settingsOrNull?.Get(WidthField), settingsOrNull?.Get(HeightField));
            if (settingsOrNull != null) IfDebugLogPair("Settings", set);

            (int, int) safe = (parms.W ?? IntOrZeroAsNull(set.W) ?? 0, parms.H ?? IntOrZeroAsNull(set.H) ?? 0);
            IfDebugLogPair("Safe", safe);


            var factorFinal = DoubleOrNullWithCalculation(factor) ?? 0;
            double arFinal = DoubleOrNullWithCalculation(aspectRatio)
                             ?? DoubleOrNullWithCalculation(settingsOrNull?.Get(AspectRatioField)) ?? 0;
            if (Debug) Log.Add($"Resize Factor: {factorFinal}, Aspect Ratio: {arFinal}");

            // if either param h/w was null, then do a rescaling on the param which comes from the settings
            // But ignore the other one!
            var rescale = (!DNearZero(factorFinal) || !DNearZero(arFinal));
            var resizedNew = rescale
                ? Rescale(safe, factorFinal, arFinal, parms.H == null)
                : safe;
            IfDebugLogPair("Rescale", resizedNew);

            var resizeParams = new ResizeSettings();
            (resizeParams.Width, resizeParams.Height) = KeepInRangeProportional(resizedNew);

            return resizeParams;
        }



        internal (int W, int H) Rescale((int W, int H) dims, double factor, double aspectRatio, bool heightNotOverriden)
        {
            var maybeLog = Debug ? Log : null;
            var wrapLog = maybeLog.SafeCall<(int, int)>();

            var useAspectRatio = !DNearZero(aspectRatio);

            // 1. Check if we have nothing to rescale
            string msgWhyNoRescale = null;
            if (dims.W == 0 && dims.H == 0) 
                msgWhyNoRescale = "w/h == 0";
            if (DNearZero(factor) || DNearZero(factor - 1)) // Factor is 0 or 1
            {
                factor = 1; // in this case we must still calculate, and should assume factor is exactly 1
                if (!useAspectRatio) msgWhyNoRescale = "Factor is 0 or 1 and no AspectRatio";
            }
            if (msgWhyNoRescale != null)
                return wrapLog(msgWhyNoRescale + ", no changes", dims);

            // 2. Figure out height/width, as we're resizing, we respect the aspect ratio, unless there is none or height shouldn't be set
            // Old: Width should only be calculated, if it wasn't explicitly provided (so only if coming from the settings)
            var newW = dims.W * factor;

            // Height should only get Aspect Ratio if the Height wasn't specifically provided
            var newH = heightNotOverriden && useAspectRatio
                ? newW / aspectRatio
                : dims.H * factor;
            
            var final = ((int)newW, (int)newH);
            return wrapLog($"W:{final.Item1}, H:{final.Item2}", final);
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
