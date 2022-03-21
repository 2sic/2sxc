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
    /// <summary>
    /// # BETA
    /// 
    /// A recipe contains instructions how to generate tags etc. which can contain multiple resized images
    /// </summary>
    /// <remarks>
    /// All the properties are read-only. If you need to override anything, copy it and set the modified values, then use the copy. 
    /// </remarks>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta / WIP")]
    public class Recipe
    {
        public const string RuleForDefault = "default";
        public const string RuleForFactor = "factor";
        public const string SpecialPropertySizes = "sizes";
        public const string SpecialPropertyMedia = "media";
        public static string[] SpecialProperties = { SpecialPropertySizes, SpecialPropertyMedia };

        /// <summary>
        /// ## Important: If you call this from your code, always use named parameters, as the parameter order can change in future
        /// </summary>
        /// <param name="original"></param>
        /// <param name="type"></param>
        /// <param name="factor"></param>
        /// <param name="width"></param>
        /// <param name="variants"></param>
        /// <param name="attributes"></param>
        /// <param name="recipes"></param>
        /// <param name="setWidth"></param>
        /// <param name="setHeight"></param>
        [JsonConstructor]   // This is important for deserialization from json
        public Recipe(
            Recipe original = null,
            // IMPORTANT: the names of these parameters may never change, as they match the names in the JSON
            string type = default, 
            string factor = default, 
            int width = default,
            string variants = default,
            Dictionary<string, object> attributes = default,
            IEnumerable<Recipe> recipes = default,
            bool? setWidth = default,
            bool? setHeight = default
        )
        {
            Type = type ?? original?.Factor ?? (factor == null ? RuleForDefault : RuleForFactor);
            Factor = factor ?? original?.Factor;
            Width = width != 0 ? width : original?.Width ?? 0;
            Variants = variants ?? original?.Variants;
            Recipes = recipes != null ? Array.AsReadOnly(recipes.ToArray()) : original?.Recipes ?? Array.AsReadOnly(Array.Empty<Recipe>());
            Attributes = attributes != null ? new ReadOnlyDictionary<string, object>(attributes) : original?.Attributes;
            SetWidth = setWidth ?? original?.SetWidth;
            SetHeight = setHeight ?? original?.SetHeight;
        }

        /// <summary>
        /// TODO: DOC
        /// - `default`, `factor`, `img`, `source`
        /// </summary>
        public string Type { get ; }

        /// <summary>
        /// TODO: DOC
        /// </summary>
        public string Factor { get; }


        /// <summary>
        /// The resize factor for which this rules is meant.
        /// Used in cases where there are many such rules and the one will be picked that matches this factor.
        /// </summary>
        //[JsonIgnore]
        [PrivateApi]
        public double FactorParsed { get; private set; }

        /// <summary>
        /// The initial width to assume in this resize, from which other sizes would be calculated.
        /// 
        /// If set to 0, it will be ignored. 
        /// </summary>
        public int Width { get; private set; }

        public bool? SetWidth { get; private set; }

        public bool? SetHeight { get; private set; }

        /// <summary>
        /// Source-Set rules (comma separated) which will determine what will be generated.
        ///
        /// Examples:
        /// - `1x,1.5x,2x` - screen resolutions
        /// - `200w,400w,600w,800w,1000w` - pixel sizes
        /// - `0.5*,1*,1.5*,2*` - multipliers of the originally specified pixel size
        /// </summary>
        public string Variants { get; private set; }


        public string Sizes => Attributes?.TryGetValue(SpecialPropertySizes, out var strSizes) == true ? strSizes as string : null;


        /// <summary>
        /// Attributes to add to the img tag 
        /// </summary>
        public ReadOnlyDictionary<string, object> Attributes { get; private set; }


        /// <summary>
        /// wip TODO: DOC
        /// </summary>
        public ReadOnlyCollection<Recipe> Recipes { get; }
        
        // TODO: CONTINUE HERE
        public ReadOnlyCollection<Recipe> RecipesFlat
        {
            get
            {
                if (_recipesFlat != null) return _recipesFlat;
                var list = new List<Recipe>();
                foreach (var r in Recipes)
                {
                    list.Add(r);
                    if(r.Recipes != null && r.Recipes.Any())
                        list.AddRange(r.Recipes);
                }

                return _recipesFlat = list.AsReadOnly();
            }
        }
        private ReadOnlyCollection<Recipe> _recipesFlat;


        [PrivateApi("Important for using these settings, but not relevant outside of this")]
        public SrcSetPart[] SrcSetParsed { get; private set; }


        internal Recipe InitAfterLoad()
        {
            if (_alreadyInit) return this;
            _alreadyInit = true;
            InitAfterLoad(null);
            return this;
        }
        private bool _alreadyInit;



        [PrivateApi]
        internal virtual Recipe InitAfterLoad(Recipe defaultsIfEmpty)
        {
            FactorParsed = ParseObject.DoubleOrNullWithCalculation(Factor) ?? defaultsIfEmpty?.FactorParsed ?? 1;
            if (Width == 0) Width = defaultsIfEmpty?.Width ?? 0;
            Variants = Variants ?? defaultsIfEmpty?.Variants;
            SetWidth = SetWidth ?? defaultsIfEmpty?.SetWidth;
            SetHeight = SetHeight ?? defaultsIfEmpty?.SetHeight;
            Attributes = Attributes ?? defaultsIfEmpty?.Attributes;
            SrcSetParsed = SrcSetParser.ParseSet(Variants);

            foreach (var s in Recipes) s?.InitAfterLoad(this);

            return this;
        }
    }
}
