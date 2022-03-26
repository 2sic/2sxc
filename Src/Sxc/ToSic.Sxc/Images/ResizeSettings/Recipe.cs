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
        [PrivateApi] public const string RuleForDefault = "default";

        // Special properties which are only added to the tag if activated in settings
        [PrivateApi] public const string SpecialPropertySizes = "sizes";
        [PrivateApi] public const string SpecialPropertyMedia = "media";
        [PrivateApi] public const string SpecialPropertyClass = "class";
        [PrivateApi] public static string[] SpecialProperties = { SpecialPropertySizes, SpecialPropertyMedia, SpecialPropertyClass };

        /// <summary>
        /// ## Important: If you call this from your code, always use named parameters, as the parameter order can change in future
        /// </summary>
        /// <param name="original"></param>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="factor"></param>
        /// <param name="width"></param>
        /// <param name="variants"></param>
        /// <param name="attributes"></param>
        /// <param name="recipes"></param>
        /// <param name="cssFramework"></param>
        /// <param name="setWidth"></param>
        /// <param name="setHeight"></param>
        [JsonConstructor]   // This is important for deserialization from json
        public Recipe(
            Recipe original = null,
            // IMPORTANT: the names of these parameters may never change, as they match the names in the JSON
            string name = default,
            string tag = default, 
            string factor = default,
            int width = default,
            string variants = default,
            Dictionary<string, object> attributes = default,
            IEnumerable<Recipe> recipes = default,
            string cssFramework = default,
            bool? setWidth = default,
            bool? setHeight = default
        )
        {
            Name = name;
            Tag = tag ?? original?.Factor ?? RuleForDefault;
            Factor = factor ?? original?.Factor;
            Width = width != 0 ? width : original?.Width ?? 0;
            Variants = variants ?? original?.Variants;
            Recipes = recipes != null ? Array.AsReadOnly(recipes.ToArray()) : original?.Recipes ?? Array.AsReadOnly(Array.Empty<Recipe>());
            Attributes = attributes != null ? new ReadOnlyDictionary<string, object>(attributes) : original?.Attributes;
            SetWidth = setWidth ?? original?.SetWidth;
            SetHeight = setHeight ?? original?.SetHeight;
            CssFramework = cssFramework;
        }


        /// <summary>
        /// Just an identifier - no technical use
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// TODO: DOC
        /// - `img`, `source`
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Determines which factors this recipe should be applied to.
        /// Null means any factor.
        /// </summary>
        public string Factor { get; private set; }


        /// <summary>
        /// WIP, not implemented yet
        /// </summary>
        public string CssFramework { get; set; }


        /// <summary>
        /// The resize factor for which this rules is meant.
        /// Used in cases where there are many such rules and the one will be picked that matches this factor.
        /// </summary>
        [PrivateApi]
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
        public string Sizes => Attributes?.TryGetValue(SpecialPropertySizes, out var strSizes) == true ? strSizes as string : null;


        /// <summary>
        /// Attributes to add to the img tag 
        /// </summary>
        public ReadOnlyDictionary<string, object> Attributes { get; private set; }


        /// <summary>
        /// wip TODO: DOC
        /// </summary>
        public ReadOnlyCollection<Recipe> Recipes { get; }
        



        [PrivateApi("Important for using these settings, but not relevant outside of this")]
        public SrcSetPart[] SrcSetParsed { get; private set; }

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
        internal virtual Recipe InitAfterLoad(Recipe defaultsIfEmpty)
        {
            Factor = Factor ?? defaultsIfEmpty?.Factor;
            FactorParsed = ParseObject.DoubleOrNullWithCalculation(Factor) ?? defaultsIfEmpty?.FactorParsed ?? 0;
            if (Width == 0) Width = defaultsIfEmpty?.Width ?? 0;
            Tag = Tag ?? defaultsIfEmpty?.Tag;
            Variants = Variants ?? defaultsIfEmpty?.Variants;
            SetWidth = SetWidth ?? defaultsIfEmpty?.SetWidth;
            SetHeight = SetHeight ?? defaultsIfEmpty?.SetHeight;
            Attributes = Attributes ?? defaultsIfEmpty?.Attributes;
            Name = Name ?? defaultsIfEmpty?.Name;
            CssFramework = CssFramework ?? defaultsIfEmpty?.CssFramework;
            SrcSetParsed = SrcSetParser.ParseSet(Variants);

            foreach (var s in Recipes) s?.InitAfterLoad(this);

            return this;
        }
    }
}
