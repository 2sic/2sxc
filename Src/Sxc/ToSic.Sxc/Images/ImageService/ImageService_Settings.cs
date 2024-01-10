using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Lib.Coding;

namespace ToSic.Sxc.Images;

partial class ImageService
{
    /// <inheritdoc />
    public IResizeSettings Settings(
        object settings = default,
        NoParamOrder noParamOrder = default,
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
        settings = GetBestSettings(settings);

        return ImgLinker.ResizeParamMerger.BuildResizeSettings(noParamOrder: noParamOrder, settings: settings, factor: factor,
            width: width, height: height, quality: quality, resizeMode: resizeMode,
            scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters, advanced: AdvancedSettings.Parse(recipe));
    }

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