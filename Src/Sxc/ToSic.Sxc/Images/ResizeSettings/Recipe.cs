using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;

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
        /// <param name="srcset"></param>
        /// <param name="attributes"></param>
        /// <param name="sub"></param>
        /// <param name="setWidth"></param>
        /// <param name="setHeight"></param>
        [JsonConstructor]   // This is important for deserialization from json
        public Recipe(
            Recipe original = null,
            // IMPORTANT: the names of these parameters may never change, as they match the names in the JSON
            string type = default, 
            string factor = default, 
            int width = default,
            string srcset = default,
            Dictionary<string, object> attributes = default,
            IEnumerable<Recipe> sub = default,
            bool? setWidth = default,
            bool? setHeight = default
        )
        {
            Type = type ?? original?.Factor ?? (factor == null ? RuleForDefault : RuleForFactor);
            Factor = factor ?? original?.Factor;
            Width = width != 0 ? width : original?.Width ?? 0;
            SrcSet = srcset ?? original?.SrcSet;
            Sub = sub != null ? Array.AsReadOnly(sub.ToArray()) : original?.Sub ?? Array.AsReadOnly(Array.Empty<Recipe>());
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
        public string SrcSet { get; private set; }

        /// <summary>
        /// Optional `sizes` attribute which would be added to `img` tags
        /// </summary>
        //public string Sizes { get; private set; }

        public string Sizes => Attributes?.TryGetValue(SpecialPropertySizes, out var strSizes) == true ? strSizes as string : null;

        ///// <summary>
        ///// Optional `media` attribute which would be added to `source` tags
        ///// </summary>
        //public string Media { get; private set; }


        /// <summary>
        /// Attributes to add to the img tag 
        /// </summary>
        public ReadOnlyDictionary<string, object> Attributes { get; private set; }


        /// <summary>
        /// wip TODO: DOC
        /// </summary>
        public ReadOnlyCollection<Recipe> Sub { get; }


        [PrivateApi("Important for using these settings, but not relevant outside of this")]
        public SrcSetPart[] SrcSetParsed { get; private set; }

        [PrivateApi]
        internal virtual Recipe InitAfterLoad(double factor, int widthIfEmpty, Recipe defaultsIfEmpty)
        {
            FactorParsed = factor;
            if (Width == 0) Width = widthIfEmpty;
            SrcSet = SrcSet ?? defaultsIfEmpty?.SrcSet;
            //Sizes = Sizes ?? defaultsIfEmpty?.Sizes;
            //Media = Media ?? defaultsIfEmpty?.Media;
            SetWidth = SetWidth ?? defaultsIfEmpty?.SetWidth;
            SetHeight = SetHeight ?? defaultsIfEmpty?.SetHeight;
            Attributes = Attributes ?? defaultsIfEmpty?.Attributes;
            SrcSetParsed = SrcSetParser.ParseSet(SrcSet);

            foreach (var s in Sub) 
                s?.InitAfterLoad(factor, widthIfEmpty, this);

            return this;
        }
    }
}
