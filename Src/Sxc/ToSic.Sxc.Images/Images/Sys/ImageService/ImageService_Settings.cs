using ToSic.Sxc.Images.Sys.ResizeSettings;
using ToSic.Sys.Utils;
using static Connect.Koi.CssFrameworks;

namespace ToSic.Sxc.Images.Sys;

partial class ImageService
{
    /// <inheritdoc />
    public IResizeSettings Settings(
        object? settings = default,
        NoParamOrder npo = default,
        Func<ITweakResize, ITweakResize>? tweak = default,
        object? factor = default,
        object? width = default,
        object? height = default,
        object? quality = default,
        string? resizeMode = default,
        string? scaleMode = default,
        string? format = default,
        object? aspectRatio = default,
        string? parameters = default,
        object? recipe = default
    ) => SettingsInternal(settings: settings, npo: npo, tweak: tweak,
        factor: factor, width: width, height: height, quality: quality,
        resizeMode: resizeMode, scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
        parameters: parameters, recipe: recipe);

    /// <summary>
    /// Internal Get-Settings, with class result (not interface).
    /// </summary>
    /// <returns>an internal settings record which could be further manipulated</returns>
    internal ResizeSettings.ResizeSettings SettingsInternal(
        object? settings = default,
        NoParamOrder npo = default,
        Func<ITweakResize, ITweakResize>? tweak = default,
        object? factor = default,
        object? width = default,
        object? height = default,
        object? quality = default,
        string? resizeMode = default,
        string? scaleMode = default,
        string? format = default,
        object? aspectRatio = default,
        string? parameters = default,
        object? recipe = default
    )
    {
        var realSettings = GetBestSettings(settings);

        var almostFinal = ImgLinker.ResizeParamMerger.BuildResizeSettings(npo: npo, settings: realSettings, factor: factor,
            width: width, height: height, quality: quality, resizeMode: resizeMode,
            scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters, advanced: AdvancedSettings.Parse(recipe));

        return (tweak?.Invoke(new TweakResize(almostFinal)) as TweakResize)?.Settings
               ?? almostFinal;
    }

    #region Settings Handling

    /// <summary>
    /// Use the given settings or try to use the default content-settings if available
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private object? GetBestSettings(object? settings)
    {
        var l = Log.Fn<object?>(enabled: Debug);
        return settings switch
        {
            null or true => l.Return(GetSettingsByName("Content"), "null/default"),
            string strName when strName.HasValue() => l.Return(GetSettingsByName(strName), $"name: {strName}"),
            _ => l.Return(settings, "unchanged")
        };
    }


    internal ICanGetByName? GetSettingsByName(string strName)
        => ResizeParamMerger.GetImageSettingsByName(ExCtxOrNull, strName, Debug, Log);

    // 2025-06 2dm - doesn't seem to be used; believe it was a feature that was never finished
    ///// <summary>
    ///// Convert to Multi-Resize Settings
    ///// </summary>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //private AdvancedSettings? ToAdv(object value) => AdvancedSettings.Parse(value);

    #endregion

    /// <inheritdoc />
    public Recipe Recipe(string variants) => new(variants: variants);

    /// <inheritdoc />
    public Recipe Recipe(
        Recipe recipe,
        NoParamOrder npo = default,
        string? name = default,
        int width = default,
        string? variants = default,
        IDictionary<string, object?>? attributes = default,
        IEnumerable<Recipe>? recipes = default,
        bool? setWidth = default,
        bool? setHeight = default,
        string? forTag = default,
        string? forFactor = default,
        string? forCss = default
    )
        => new(recipe, name: name, width: width, variants: variants, attributes: attributes, recipes: recipes, 
            setWidth: setWidth, setHeight: setHeight, forTag: forTag, forFactor: forFactor, forCss: forCss);

    internal string? OverrideCssFramework
    {
        get
        {
            // If Koi knows the framework, then no need to check features
            if (!ImgLinker.Koi.IsUnknown)
                return null;

            // Get list of features, and only re-check if the count changed
            var fka = PageService.FeatureKeysAdded;
            if (fka.Count == _lastCheckCount)
                return field;
            _lastCheckCount = fka.Count;

            // Determine framework override
            // Note that BootstrapX is more of a test flag, so check it first
            return field = fka.Contains("BootstrapX") ? "bsX"
                : fka.Contains(nameof(Bootstrap6)) ? Bootstrap6
                : fka.Contains(nameof(Bootstrap5)) ? Bootstrap5
                : null;
        }
    }

    private int _lastCheckCount;
}