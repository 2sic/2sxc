using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Images;

partial class ImageService
{
    /// <inheritdoc />
    public IResizeSettings Settings(
        object settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakResize, ITweakResize> tweak = default,
        object factor = default,
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string parameters = default,
        object recipe = default
    ) => SettingsInternal(settings: settings, noParamOrder: noParamOrder, tweak: tweak,
        factor: factor, width: width, height: height, quality: quality,
        resizeMode: resizeMode, scaleMode: scaleMode, format: format, aspectRatio: aspectRatio,
        parameters: parameters, recipe: recipe);

    /// <summary>
    /// Internal Get-Settings, with internal type.
    /// </summary>
    /// <returns>an internal settings record which could be further manipulated</returns>
    internal ResizeSettings SettingsInternal(
        object settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakResize, ITweakResize> tweak = default,
        object factor = default,
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string parameters = default,
        object recipe = default
    )
    {
        var realSettings = GetBestSettings(settings);

        var almostFinal = ImgLinker.ResizeParamMerger.BuildResizeSettings(noParamOrder: noParamOrder, settings: realSettings, factor: factor,
            width: width, height: height, quality: quality, resizeMode: resizeMode,
            scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters, advanced: AdvancedSettings.Parse(recipe));

        return tweak != null 
            ? (tweak?.Invoke(new TweakResize(almostFinal)) as TweakResize)?.Settings ?? almostFinal
            : almostFinal;
    }

    #region Settings Handling

    /// <summary>
    /// Use the given settings or try to use the default content-settings if available
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private object GetBestSettings(object settings)
    {
        var l = Log.Fn<object>(enabled: Debug);
        return settings switch
        {
            null or true => l.Return(GetSettingsByName("Content"), "null/default"),
            string strName when strName.HasValue() => l.Return(GetSettingsByName(strName), $"name: {strName}"),
            _ => l.Return(settings, "unchanged")
        };
    }


    internal ICanGetByName GetSettingsByName(string strName)
        => ResizeParamMerger.GetImageSettingsByName(_CodeApiSvc, strName, Debug, Log);

    /// <summary>
    /// Convert to Multi-Resize Settings
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private AdvancedSettings ToAdv(object value) => AdvancedSettings.Parse(value);

    #endregion

    /// <inheritdoc />
    public Recipe Recipe(string variants) => new(variants: variants);

    /// <inheritdoc />
    public Recipe Recipe(
        Recipe recipe,
        NoParamOrder noParamOrder = default,
        string name = default,
        int width = default,
        string variants = default,
        IDictionary<string, object> attributes = default,
        IEnumerable<Recipe> recipes = default,
        bool? setWidth = default,
        bool? setHeight = default,
        string forTag = default,
        string forFactor = default,
        string forCss = default
    )
        => new(recipe, name: name, width: width, variants: variants, attributes: attributes, recipes: recipes, 
            setWidth: setWidth, setHeight: setHeight, forTag: forTag, forFactor: forFactor, forCss: forCss);
}