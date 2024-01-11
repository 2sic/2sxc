using System.Text.Json.Serialization;
using ToSic.Sxc.Internal.Plumbing;


namespace ToSic.Sxc.Images;

/// <summary>
/// A recipe contains instructions how to generate tags etc. which can contain multiple resized images
/// </summary>
/// <remarks>
/// All the properties are read-only. If you need to override anything, copy it and set the modified values, then use the copy. 
/// </remarks>
/// <remarks>
/// History: Released 2sxc 13.10
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta / WIP")]   // note: was marked as PublicApi(Beta/WIP) till v17
public class Recipe: ICanDump
{
    internal const string RuleForDefault = "default";

    // Special properties which are only added to the tag if activated in settings
    internal const string SpecialPropertySizes = "sizes";
    internal const string SpecialPropertyMedia = "media";
    internal const string SpecialPropertyClass = "class";
    internal const string SpecialPropertyStyle = "style";
    internal static string[] SpecialProperties = [SpecialPropertySizes, SpecialPropertyMedia, SpecialPropertyClass, SpecialPropertyStyle];


    /// <summary>
    /// **Important**
    ///
    /// If you call this from your code, always use named parameters, as the parameter order can change in future.
    /// </summary>
    /// <param name="original">An original recipe to copy if we want to get a modified recipe based on one which already existed.</param>
    /// <param name="name">An optional name </param>
    /// <param name="forTag">Restricts the rule to only apply to specific tags - ATM `img` and `source`</param>
    /// <param name="forFactor">Restricts the rule to only apply to resizes for a specified factor</param>
    /// <param name="forCss">Restricts the rule to only apply to pages which have the specified CSS Framework</param>
    /// <param name="width">Initial width to use when resizing</param>
    /// <param name="setWidth">Set the `width` attribute if the img width is known</param>
    /// <param name="setHeight">Set the `height` attribute if the img-height is known</param>
    /// <param name="variants">Special string containing variants to generate</param>
    /// <param name="attributes">List of attributes to set on the `img` tag</param>
    /// <param name="recipes">List of additional recipes which will all inherit values from this master after creation</param>
    public Recipe(
        Recipe original,
        // IMPORTANT: the names of these parameters may never change, as they match the names in the JSON
        string name = default,
        string forTag = default,
        string forFactor = default,
        string forCss = default,
        int width = default,
        bool? setWidth = default,
        bool? setHeight = default,
        string variants = default,
        IDictionary<string, object> attributes = null,
        IEnumerable<Recipe> recipes = default
    )
    {
        Name = name;
        ForTag = forTag ?? original?.ForFactor ?? RuleForDefault;
        ForFactor = forFactor ?? original?.ForFactor;
        ForCss = forCss;
        Width = width != 0 ? width : original?.Width ?? 0;
        SetWidth = setWidth ?? original?.SetWidth;
        SetHeight = setHeight ?? original?.SetHeight;
        Variants = variants ?? original?.Variants;
        Recipes = recipes != null ? Array.AsReadOnly(recipes.ToArray()) : original?.Recipes ?? Array.AsReadOnly(Array.Empty<Recipe>());
        Attributes = RecipeHelpers.MergeDics(original?.Attributes, attributes);
    }

    /// <summary>
    /// Lighter constructor for json, without parameter Recipe.
    /// 
    /// **Important**
    /// If you call this from your code, always use named parameters, as the parameter order can change in future.
    /// </summary>
    /// <param name="name">An optional name </param>
    /// <param name="forTag">Restricts the rule to only apply to specific tags - ATM `img` and `source`</param>
    /// <param name="forFactor">Restricts the rule to only apply to resizes for a specified factor</param>
    /// <param name="forCss">Restricts the rule to only apply to pages which have the specified CSS Framework</param>
    /// <param name="width">Initial width to use when resizing</param>
    /// <param name="setWidth">Set the `width` attribute if the img width is known</param>
    /// <param name="setHeight">Set the `height` attribute if the img-height is known</param>
    /// <param name="variants">Special string containing variants to generate</param>
    /// <param name="attributes">List of attributes to set on the `img` tag</param>
    /// <param name="recipes">List of additional recipes which will all inherit values from this master after creation</param>
    [JsonConstructor]   // This is important for deserialization from json
    public Recipe(
        // IMPORTANT: the names of these parameters may never change, as they match the names in the JSON
        string name = default,
        string forTag = default,
        string forFactor = default,
        string forCss = default,
        int width = default,
        bool? setWidth = default,
        bool? setHeight = default,
        string variants = default,
        IDictionary<string, object> attributes = null,
        IEnumerable<Recipe> recipes = default
    ) : this(original: null, name: name, forTag: forTag, forFactor: forFactor, forCss: forCss, width: width, setWidth: setWidth, setHeight: setHeight, variants: variants, attributes: attributes, recipes: recipes)
    { }


    /// <summary>
    /// Just an identifier - no technical use
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// TODO: DOC
    /// - `img`, `source`
    /// </summary>
    public string ForTag { get; private set; }

    /// <summary>
    /// Determines which factors this recipe should be applied to.
    /// Null means any factor.
    /// </summary>
    public string ForFactor { get; private set; }


    /// <summary>
    /// WIP, not implemented yet
    /// </summary>
    public string ForCss { get; set; }


    /// <summary>
    /// The resize factor for which this rules is meant.
    /// Used in cases where there are many such rules and the one will be picked that matches this factor.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public double FactorParsed { get; private set; }

    /// <summary>
    /// The initial width to assume in this resize, from which other sizes would be calculated.
    /// 
    /// If set to 0, it will be ignored. 
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Determines if the img tag will receive a width-attribute
    /// </summary>
    public bool? SetWidth { get; private set; }

    /// <summary>
    /// Determines if the img tag will receive a height-attribute
    /// </summary>
    public bool? SetHeight { get; private set; }

    /// <summary>
    /// Source-Set rules (comma separated) which will determine what will be generated.
    ///
    /// Examples:
    /// - `1x,1.5x,2x` - screen resolutions
    /// - `200w,400w,600w,800w,1000w` - pixel sizes
    /// - `0.5*,1*,1.5*,2*` - multipliers of the originally specified pixel size
    ///
    /// _Important: According to the HTML standard you can mix pixel-sizes and multipliers, but not resolutions with any of the other types._
    /// </summary>
    public string Variants { get; private set; }

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public string Sizes => Attributes?.TryGetValue(SpecialPropertySizes, out var strSizes) == true ? strSizes.ToString() : null;


    /// <summary>
    /// Attributes to add to the img tag 
    /// </summary>
    /// <remarks>
    /// System.Text.Json requires that the case-insensitive property name and type match the parameter in the constructor.
    /// We are using string/object because a value could also be { "name": true, "other-name": 5 } in the json configuration
    /// </remarks>
    public IDictionary<string, object> Attributes { get; private set; }

    /// <summary>
    /// wip TODO: DOC
    /// </summary>
    /// <remarks>
    /// System.Text.Json requires that the case-insensitive property name and type match the parameter in the constructor.
    /// </remarks>
    public IEnumerable<Recipe> Recipes { get; } 


    [PrivateApi("Important for using these settings, but not relevant outside of this")]
    internal RecipeVariant[] VariantsParsed { get; private set; }

    [PrivateApi]
    internal Recipe InitAfterLoad()
    {
        if (_alreadyInit) return this;
        _alreadyInit = true;
        InitAfterLoad(null);
        return this;
    }
    private bool _alreadyInit;



    [PrivateApi]
    internal virtual Recipe InitAfterLoad(Recipe parent)
    {
        ForFactor ??= parent?.ForFactor;
        FactorParsed = ParseObject.DoubleOrNullWithCalculation(ForFactor) ?? parent?.FactorParsed ?? 0;
        if (Width == 0) Width = parent?.Width ?? 0;
        ForTag ??= parent?.ForTag;
        var hasVariants = Variants != null;
        Variants ??= parent?.Variants;
        SetWidth ??= parent?.SetWidth;
        SetHeight ??= parent?.SetHeight;
        Attributes = RecipeHelpers.MergeDics(parent?.Attributes, Attributes);
        Name ??= parent?.Name;
        ForCss ??= parent?.ForCss;
        VariantsParsed = hasVariants ? RecipeVariantsParser.ParseSet(Variants) : parent?.VariantsParsed;

        foreach (var s in Recipes) s?.InitAfterLoad(this);

        return this;
    }

    public string Dump() =>
        $"{nameof(Name)}: '{Name}', {nameof(ForCss)}: {ForCss}, " +
        $"{nameof(ForFactor)}: {ForFactor}, {nameof(FactorParsed)}: {FactorParsed}, " +
        $"{nameof(Variants)}: '{Variants}', {nameof(VariantsParsed)}: {VariantsParsed}, " +
        $"{nameof(SetWidth)}: {SetWidth}, {nameof(SetHeight)}: {SetHeight}, " +
        $"{nameof(Attributes)}: {Attributes?.Count}";
}