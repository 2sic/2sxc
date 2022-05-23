using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class AdvancedSettings: IHasPiggyBack
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
        [JsonIgnore]
        internal Recipe Recipe { get; }

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
                return new AdvancedSettings(mrrValue);

            return null;
        }

        [PrivateApi]
        public static AdvancedSettings FromJson(object value, ILog log = null)
        {
            var wrapLog = log.Fn<AdvancedSettings>();
            try
            {
                if (value is string advString && !string.IsNullOrWhiteSpace(advString))
                    return wrapLog.Return(JsonConvert.DeserializeObject<AdvancedSettings>(advString), "create");
            }
            catch (Exception ex)
            {
                log.A($"error converting json to AdvancedSettings. Json: {value}");
                log?.Ex(ex);
            }
            return wrapLog.Return(new AdvancedSettings(), "new");
        }

        [PrivateApi]
        public ReadOnlyCollection<Recipe> AllSubRecipes
            => _recipesFlat ?? (_recipesFlat = GetAllRecipesRecursive(Recipe?.Recipes).AsReadOnly());
        private ReadOnlyCollection<Recipe> _recipesFlat;

        private static List<Recipe> GetAllRecipesRecursive(ReadOnlyCollection<Recipe> recipes)
        {
            var list = new List<Recipe>();
            if (recipes?.Any() != true) return list;

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
        public PiggyBack PiggyBack => _piggyBack ?? (_piggyBack = new PiggyBack());
        private PiggyBack _piggyBack;

    }
}
