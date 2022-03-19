using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Plumbing;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class RecipeSet
    {
        public RecipeSet(Recipe @default = default, IEnumerable<Recipe> recipes = default)
        {
            Default = @default != null ? new Recipe(@default, type: Recipe.RuleForDefault) : null;
            Recipes = Array.AsReadOnly(recipes?.ToArray() ?? Array.Empty<Recipe>());
        }

        /// <summary>
        /// Default Resize rules for everything which isn't specified more closely in the factors
        /// </summary>
        [JsonIgnore]
        internal Recipe Default { get; }

        [JsonProperty("recipes")]
        public ReadOnlyCollection<Recipe> Recipes { get; private set; }
    
        // TODO: MERGE INTO CONSTRUCTOR
        // TODO: Make sure all recipes also contain default ?
        // TODO: add recipeSets, remove "sub" properties
        [PrivateApi]
        public RecipeSet InitAfterLoad()
        {
            if (_alreadyInit) return this;
            _alreadyInit = true;

            // Ensure Factors not null
            //Recipes = Recipes ?? Array.AsReadOnly(Array.Empty<Recipe>());

            // Init Default first
            Default?.InitAfterLoad(1, 0, null);

            Recipes = Array.AsReadOnly(InitFactors());
            return this;
        }
        private bool _alreadyInit;

        private Recipe[] InitFactors()
        {
            // Drop null-rules
            var rules = Recipes.Where(fr => fr != default).ToArray();

            // Only init non-defaults, as that should only exist once and was already initialized
            foreach (var r in rules) // .Where(r => r != Default))
            {
                var factor = ParseObject.DoubleOrNullWithCalculation(r.Factor);
                r.InitAfterLoad(factor ?? 0, r.Width, Default);
            }
            return rules;
        }

        public static RecipeSet Parse(object value) => InnerParse(value)?.InitAfterLoad();

        private static RecipeSet InnerParse(object value)
        {
            if (value == null) return null;

            // It's already what's expected
            if (value is RecipeSet mrsValue) return mrsValue;

            // Parse any string which would be a typical MRS - convert to single rule
            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                value = new Recipe(srcset: strValue);

            // Parse any single rule It's just one rule which should be used
            if (value is Recipe mrrValue)
                return new RecipeSet(mrrValue);

            return null;
        }
    }
}
