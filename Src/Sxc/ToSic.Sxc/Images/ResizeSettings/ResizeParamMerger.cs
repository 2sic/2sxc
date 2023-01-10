using System;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using static ToSic.Sxc.Images.ImageConstants;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// This merges predefined settings with custom specified parameters to create a stable resize-Parameters object for further use
    /// </summary>
    internal class ResizeParamMerger: HelperBase
    {
        private const string ResizeModeField = "ResizeMode";
        private const string ScaleModeField = "ScaleMode";
        private const string QualityField = "Quality";
        private const string WidthField = "Width";
        private const string HeightField = "Height";
        private const string AspectRatioField = "AspectRatio";
        private const string AdvancedField = "Advanced";

        public ResizeParamMerger(ILog parentLog) : base(parentLog, $"{Constants.SxcLogName}.ImgRPM") { }

        public bool Debug = false;

        internal ResizeSettings BuildResizeSettings(
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string parameters = null,
            AdvancedSettings advanced = default
        )
        {
            var wrapLog = Log.Fn<ResizeSettings>(Debug);
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(BuildResizeSettings)}", $"{nameof(settings)},{nameof(factor)},{nameof(width)}, ...");

            // check common mistakes
            if (aspectRatio != null && height != null)
            {
                wrapLog.ReturnNull("error");
                const string messageOnlyOneOrNone = "only one or none of these should be provided, other can be zero";
                throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);
            }

            // Helper for resize parameters
            var resP = new ResizeParams(Log);

            if (settings is IResizeSettings typeSettings)
            {
                wrapLog.A(Debug, $"Is {nameof(ResizeSettings)}, will clone/init");
                return new ResizeSettings(
                    typeSettings,
                    format: resP.FormatOrNull(format),
                    width: resP.WidthOrNull(width),
                    height: resP.HeightOrNull(height),
                    aspectRatio: resP.AspectRatioOrNull(aspectRatio),
                    factor: resP.FactorOrNull(factor),
                    quality: resP.QualityOrNull(quality),
                    resizeMode: resP.ResizeModeOrNull(resizeMode),
                    scaleMode: resP.ScaleModeOrNull(scaleMode),
                    parameters: resP.ParametersOrNull(parameters),
                    advanced: advanced
                );
            }

            // Check if the settings is the expected type or null/other type
            var getSettings = settings as ICanGetByName;
            wrapLog.A(Debug, $"Has Settings:{getSettings != null}");


            var formatValue = resP.FormatOrNull(format);

            var resizeParams = BuildCoreSettings(resP, width, height, factor, aspectRatio, formatValue, getSettings);

            // Add parameters if known
            resizeParams.Parameters = resP.ParametersOrNull(parameters);

            // Aspects which aren't affected by scale
            var qParamInt2 = resP.QualityOrNull(quality);
            resizeParams.Quality = qParamInt2 ?? IntOrZeroAsNull(getSettings?.Get(QualityField)) ?? IntIgnore;
            resizeParams.ResizeMode = resP.ResizeModeOrNull(KeepBestString(resizeMode, getSettings?.Get(ResizeModeField)));
            resizeParams.ScaleMode = resP.ScaleModeOrNull(KeepBestString(scaleMode, getSettings?.Get(ScaleModeField)));

            resizeParams.Advanced = GetMultiResizeSettings(advanced, getSettings);

            return wrapLog.ReturnAsOk(resizeParams);
        }

        private AdvancedSettings GetMultiResizeSettings(AdvancedSettings advanced, ICanGetByName getSettings)
        {
            if (advanced != null) return advanced;
            AdvancedSettings ParseAdvancedSettingsJson(object value) => AdvancedSettings.FromJson(value, Log);
            try
            {
                // Check if we have a property-lookup (usually an entity) and if yes, use the piggy-back
                if (getSettings is IPropertyLookup getProperties)
                {
                    var result = getProperties.GetOrCreateInPiggyBack(AdvancedField, ParseAdvancedSettingsJson, Log);
                    if (result != null) return result;
                }

                return ParseAdvancedSettingsJson(getSettings?.Get(AdvancedField));
            }
            catch
            {
                /* ignore */
            }

            return null;
        }

        internal ResizeSettings BuildCoreSettings(ResizeParams resP, object width, object height, object factor, object aspectRatio, string format, ICanGetByName settingsOrNull)
        {
            void IfDebugLogPair<T>(string prefix, (T W, T H) values)
            {
                if (Debug) Log.A($"{prefix}: W:{values.W}, H:{values.H}");
            }

            // Try to pre-process parameters and prefer them
            // The manually provided values must remember Zeros because they deactivate presets
            (int? W, int? H) parameters = (resP.WidthOrNull(width), resP.HeightOrNull(height));
            IfDebugLogPair("Params", parameters);

            // Pre-Clean the values - all as strings
            (dynamic W, dynamic H) set = (settingsOrNull?.Get(WidthField), settingsOrNull?.Get(HeightField));
            if (settingsOrNull != null) IfDebugLogPair("Settings", set);

            (int W, int H) safe = (IntOrZeroAsNull(set.W) ?? IntIgnore, IntOrZeroAsNull(set.H) ?? IntIgnore);
            IfDebugLogPair("Safe", safe);

            var factorFinal = resP.FactorOrNull(factor) ?? IntIgnore;
            double arFinal = resP.AspectRatioOrNull(aspectRatio)
                             ?? resP.AspectRatioOrNull(settingsOrNull?.Get(AspectRatioField)) ?? IntIgnore;
            Log.A(Debug, $"Resize Factor: {factorFinal}, Aspect Ratio: {arFinal}");

            var resizeSettings = new ResizeSettings(parameters.W, parameters.H,
                safe.W, safe.H,
                arFinal, factorFinal, format);

            return resizeSettings;
        }
        
    }
}
