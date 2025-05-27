using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Serialization;
using ToSic.Lib.Caching.PiggyBack;

namespace ToSic.Sxc.Images;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdvancedSettings : IHasPiggyBack
{
    [JsonConstructor]
    public AdvancedSettings(Recipe recipe = default)
    {
        Recipe = recipe;

        // Initialize the recipe and all sub-recipes
        Recipe?.InitAfterLoad();
    }

    /// <summary>
    /// Default Resize rules for everything which isn't specified more closely in the factors
    /// </summary>
    /// <remarks>
    /// System.Text.Json requires that the case-insensitive property name and type match the parameter in the constructor.
    /// </remarks>
    public Recipe Recipe { get; }

    [PrivateApi]
    public static AdvancedSettings Parse(object value) => InnerParse(value);

    private static AdvancedSettings InnerParse(object value)
    {
        if (value == null) return null;

        // It's already what's expected
        if (value is AdvancedSettings mrsValue) return mrsValue;

        // Parse any string which would be a typical MRS - convert to single rule
        if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
            value = new Recipe(variants: strValue);

        // Parse any single rule It's just one rule which should be used
        if (value is Recipe mrrValue)
            return new(mrrValue);

        return null;
    }

    [PrivateApi]
    public static AdvancedSettings FromJson(object value, ILog log = null)
    {
        var l = log.Fn<AdvancedSettings>();
        try
        {
            if (value is string advString && !string.IsNullOrWhiteSpace(advString))
                return l.Return(
                    JsonSerializer.Deserialize<AdvancedSettings>(advString,
                        JsonOptions.UnsafeJsonWithoutEncodingHtml), "create");
        }
        catch (Exception ex)
        {
            l.A($"error converting json to AdvancedSettings. Json: {value}");
            l.Ex(ex);
        }

        return l.Return(new(), "new");
    }

    [PrivateApi]
    public ReadOnlyCollection<Recipe> AllSubRecipes => field ??= GetAllRecipesRecursive(Recipe?.Recipes).AsReadOnly();

    private static List<Recipe> GetAllRecipesRecursive(IEnumerable<Recipe> recipes)
    {
        var list = new List<Recipe>();
        if (recipes.SafeNone()) return list;

        foreach (var r in recipes)
        {
            list.Add(r);
            list.AddRange(GetAllRecipesRecursive(r.Recipes));
        }

        return list;
    }


    /// <summary>
    /// Piggyback cache to remember previous LINQ queries which already filtered certain combinations
    /// </summary>
    [PrivateApi("internal use only")]
    [JsonIgnore]
    public PiggyBack PiggyBack => field ??= new();
}