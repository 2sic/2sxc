using Newtonsoft.Json;
using ToSic.Eav.Documentation;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class RecipeSet
    {
        [JsonConstructor]
        public RecipeSet(Recipe recipe = default)
        {
            Recipe = recipe != null ? new Recipe(recipe, tag: Recipe.RuleForDefault) : null;
        }

        /// <summary>
        /// Default Resize rules for everything which isn't specified more closely in the factors
        /// </summary>
        [JsonIgnore]
        internal Recipe Recipe { get; }

    
        // Important: Never merge into the constructor
        // TODO: Make sure all recipes also contain default ?
        // TODO: add recipeSets, remove "sub" properties
        [PrivateApi]
        public RecipeSet InitAfterLoad()
        {
            if (_alreadyInit) return this;
            _alreadyInit = true;

            // Init Default first
            Recipe?.InitAfterLoad();
            return this;
        }
        private bool _alreadyInit;


        public static RecipeSet Parse(object value) => InnerParse(value)?.InitAfterLoad();

        private static RecipeSet InnerParse(object value)
        {
            if (value == null) return null;

            // It's already what's expected
            if (value is RecipeSet mrsValue) return mrsValue;

            // Parse any string which would be a typical MRS - convert to single rule
            if (value is string strValue && !string.IsNullOrWhiteSpace(strValue))
                value = new Recipe(variants: strValue);

            // Parse any single rule It's just one rule which should be used
            if (value is Recipe mrrValue)
                return new RecipeSet(mrrValue);

            return null;
        }
    }
}
