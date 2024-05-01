using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal;
using static ToSic.Sxc.Images.Internal.ImageConstants;
using static ToSic.Sxc.Internal.Plumbing.ParseObject;

namespace ToSic.Sxc.Images;

/// <summary>
/// This merges predefined settings with custom specified parameters to create a stable resize-Parameters object for further use
/// </summary>
internal class ResizeParamMerger(ILog parentLog) : HelperBase(parentLog, $"{SxcLogName}.ImgRPM")
{
    private const string ResizeModeField = "ResizeMode";
    private const string ScaleModeField = "ScaleMode";
    private const string QualityField = "Quality";
    private const string WidthField = "Width";
    private const string HeightField = "Height";
    private const string AspectRatioField = "AspectRatio";
    private const string AdvancedField = "Advanced";

    public bool Debug = false;

    internal ResizeSettings BuildResizeSettings(
        object settings = null,
        NoParamOrder noParamOrder = default,
        object factor = null,
        object width = null,
        object height = null,
        object quality = null,
        string resizeMode = null,
        string scaleMode = null,
        string format = null,
        object aspectRatio = null,
        string parameters = null,
        AdvancedSettings advanced = default,
        ICodeApiService codeApiSvc = default
    )
    {
        var l = (Debug ? Log : null).Fn<ResizeSettings>();
        // Common mistake: both height and aspect ratio provided
        if (aspectRatio != null && height != null)
        {
            l.ReturnNull("error");
            const string messageOnlyOneOrNone = "only one or none of these should be provided, other can be zero";
            throw new ArgumentOutOfRangeException($"{nameof(aspectRatio)},{nameof(height)}", messageOnlyOneOrNone);
        }

        // Helper for resize parameters
        var paramHelper = new ResizeParams(Log);

        // If we already got resize settings, then clone/merge
        if (settings is IResizeSettings typeSettings)
            return l.Return(new(
                typeSettings,
                format: paramHelper.FormatOrNull(format),
                width: paramHelper.WidthOrNull(width),
                height: paramHelper.HeightOrNull(height),
                aspectRatio: paramHelper.AspectRatioOrNull(aspectRatio),
                factor: paramHelper.FactorOrNull(factor),
                quality: paramHelper.QualityOrNull(quality),
                resizeMode: paramHelper.ResizeModeOrNull(resizeMode),
                scaleMode: paramHelper.ScaleModeOrNull(scaleMode),
                parameters: paramHelper.ParametersOrNull(parameters),
                advanced: advanced
            ), $"Is {nameof(ResizeSettings)}, will clone/init");

        // Check if the settings is the expected type or null/other type
        var settingsOrNull = TryToCastSettings(settings, codeApiSvc);
        l.A($"Has Settings: {settingsOrNull != null}; type: {settings?.GetType().FullName}");

        var formatValue = paramHelper.FormatOrNull(format);

        var resizeParams = BuildCoreSettings(paramHelper, width, height, factor, aspectRatio, formatValue, settingsOrNull);

        // Add more URL parameters if known
        resizeParams.Parameters = paramHelper.ParametersOrNull(parameters);

        // Aspects which aren't affected by aspect ratio
        var qParamInt2 = paramHelper.QualityOrNull(quality);
        resizeParams.Quality = qParamInt2 ?? IntOrZeroAsNull(settingsOrNull?.Get(QualityField)) ?? IntIgnore;
        resizeParams.ResizeMode =
            paramHelper.ResizeModeOrNull(KeepBestString(resizeMode, settingsOrNull?.Get(ResizeModeField)));
        resizeParams.ScaleMode = paramHelper.ScaleModeOrNull(KeepBestString(scaleMode, settingsOrNull?.Get(ScaleModeField)));

        resizeParams.Advanced = GetMultiResizeSettings(advanced, settingsOrNull);

        return l.Return(resizeParams, "");
    }

    private ICanGetByName TryToCastSettings(object settings, ICodeApiService codeApiServiceOrNull) =>
        settings switch
        {
            null => null,
            string name => GetImageSettingsByName(codeApiServiceOrNull, name, Debug, Log) as ICanGetByName,
            ICanGetByName getSettings => getSettings,
            IEnumerable<ICanGetByName> settingsList => settingsList.FirstOrDefault(),
            _ => null
        };

    internal static object GetImageSettingsByName(ICodeApiService codeApiSvcOrNull, string strName, bool debug, ILog log)
    {
        var l = debug ? log.Fn<object>($"{strName}; code root: {codeApiSvcOrNull != null}") : null;
        var result = codeApiSvcOrNull?.Settings?.Get($"Settings.Images.{strName}");
        return l.Return((object)result, $"found: {result != null}");
    }



    private AdvancedSettings GetMultiResizeSettings(AdvancedSettings advanced, ICanGetByName getSettings)
    {
        if (advanced != null) return advanced;
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

        AdvancedSettings ParseAdvancedSettingsJson(object value) => AdvancedSettings.FromJson(value, Log);
    }

    internal ResizeSettings BuildCoreSettings(ResizeParams resP, object width, object height, object factor, object aspectRatio, string format, ICanGetByName settingsOrNull)
    {
        var l = (Debug ? Log : null).Fn<ResizeSettings>();

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
        var arFinal = resP.AspectRatioOrNull(aspectRatio)
                      ?? resP.AspectRatioOrNull(settingsOrNull?.Get(AspectRatioField)) ?? IntIgnore;
        l.A(Debug, $"Resize Factor: {factorFinal}, Aspect Ratio: {arFinal}");

        var basedOnName = settingsOrNull?.Get("ItemIdentifier") as string;

        var resizeSettings = new ResizeSettings(width: parameters.W, height: parameters.H,
            fallbackWidth: safe.W, fallbackHeight: safe.H,
            aspectRatio: arFinal, factor: factorFinal, format: format, basedOn: basedOnName);

        return l.Return(resizeSettings);

        // Helper to debug
        void IfDebugLogPair<T>(string prefix, (T W, T H) values)
        {
            if (Debug) Log.A($"{prefix}: W:{values.W}, H:{values.H}");
        }
    }
        
}