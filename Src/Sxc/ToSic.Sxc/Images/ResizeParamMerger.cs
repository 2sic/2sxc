using System;
using Newtonsoft.Json;
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
        private const string AdvancedField = "Advanced";

        public ResizeParamMerger() : base(Constants.SxcLogName + ".ImgRPM") { }

        public bool Debug = false;


        internal ResizeSettings BuildResizeSettings(
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
            object srcset = null,
            object advanced = null
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

            var formatValue = FindKnownFormatOrNull(RealStringOrNull(format));
            string srcSetValue = srcset is string srcSetString
                ? srcSetString
                : srcset is bool srcSetBool && srcSetBool ? getSettings?.Get(SrcSetField) : null;


            var resizeParams = BuildCoreSettings(width, height, factor, aspectRatio, formatValue, srcSetValue, getSettings);

            // Add parameters if known
            if (!string.IsNullOrWhiteSpace(parameters))
                resizeParams.Parameters = UrlHelpers.ParseQueryString(parameters);

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

            try
            {
                // Use given OR
                if (advanced == null || advanced is string strAdvanced && string.IsNullOrWhiteSpace(strAdvanced))
                    advanced = getSettings?.Get(AdvancedField);

                if (advanced is ResizeSettingsAdvanced advTyped)
                    resizeParams.Advanced = advTyped;
                else if (advanced is string advString && !string.IsNullOrWhiteSpace(advString))
                    resizeParams.Advanced = JsonConvert.DeserializeObject<ResizeSettingsAdvanced>(advString);
            }
            catch{ /* ignore */ }

            return resizeParams;
        }

        internal ResizeSettings BuildCoreSettings(object width, object height, object factor, object aspectRatio, string format, string srcSet, ICanGetNameNotFinal settingsOrNull)
        {
            // Try to pre-process parameters and prefer them
            // The manually provided values must remember Zeros because they deactivate presets
            (int? W, int? H) parms = (IntOrNull(width), IntOrNull(height));
            IfDebugLogPair("Params", parms);

            // Pre-Clean the values - all as strings
            (dynamic W, dynamic H) set = (settingsOrNull?.Get(WidthField), settingsOrNull?.Get(HeightField));
            if (settingsOrNull != null) IfDebugLogPair("Settings", set);

            (int W, int H) safe = (parms.W ?? IntOrZeroAsNull(set.W) ?? 0, parms.H ?? IntOrZeroAsNull(set.H) ?? 0);
            IfDebugLogPair("Safe", safe);

            var factorFinal = DoubleOrNullWithCalculation(factor) ?? 0; // 0 = ignore
            double arFinal = DoubleOrNullWithCalculation(aspectRatio)
                             ?? DoubleOrNullWithCalculation(settingsOrNull?.Get(AspectRatioField)) ?? 0; // 0=ignore
            if (Debug) Log.Add($"Resize Factor: {factorFinal}, Aspect Ratio: {arFinal}");

            var resizeSettings = new ResizeSettings(safe.W, safe.H, arFinal, factorFinal, format, srcSet)
            {
                // If the width was given by the parameters, then don't use FactorMap
                UseFactorMap = parms.W != null,
                // If the height was supplied by parameters, don't use aspect ratio
                UseAspectRatio = parms.H == null,
            };

            return resizeSettings;
        }
        

        protected void IfDebugLogPair<T>(string prefix, (T W, T H) values)
        {
            if (Debug) Log.Add($"{prefix}: W:{values.W}, H:{values.H}");
        }

    }
}
