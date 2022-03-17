using System;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// # BETA
    /// 
    /// Rule how to generate tags etc. which can contain multiple resized images
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta / WIP")]
    public class MultiResizeRule
    {
        public const string JsonType = "type";
        public const string JsonFactor = "factor";
        public const string JsonWidth = "width";
        public const string JsonSrcset = "srcset";
        public const string JsonSizes = "sizes";
        public const string JsonMedia = "media";
        public const string JsonSub = "sub";
        public const string RuleForDefault = "default";
        public const string RuleForFactor = "factor";

        [JsonProperty(JsonType)]
        public string Type
        {
            get => _for ?? (Factor == null ? RuleForDefault : RuleForFactor);
            set => _for = value;
        }
        private string _for; // Default is null = not set, auto-detect

        [JsonProperty(JsonFactor)]
        public string Factor { get; set; }


        /// <summary>
        /// The resize factor for which this rules is meant.
        /// Used in cases where there are many such rules and the one will be picked that matches this factor.
        /// </summary>
        [JsonIgnore]
        public double FactorParsed { get; set; }

        /// <summary>
        /// The initial width to assume in this resize, from which other sizes would be calculated.
        /// 
        /// If set to 0, it will be ignored. 
        /// </summary>
        [JsonProperty(JsonWidth)]
        public int Width { get; set; }

        /// <summary>
        /// Source-Set rules (comma separated) which will determine what will be generated.
        ///
        /// Examples:
        /// - `1x,1.5x,2x` - screen resolutions
        /// - `200w,400w,600w,800w,1000w` - pixel sizes
        /// - `0.5*,1*,1.5*,2*` - multipliers of the originally specified pixel size
        /// </summary>
        [JsonProperty(JsonSrcset)]
        public string SrcSet { get; set; }

        /// <summary>
        /// Optional `sizes` attribute which would be added to `img` tags
        /// </summary>
        [JsonProperty(JsonSizes)]
        public string Sizes { get; set; }

        /// <summary>
        /// Optional `media` attribute which would be added to `source` tags
        /// </summary>
        [JsonProperty(JsonMedia)]
        public string Media { get; set; }

        [JsonProperty(JsonSub)]
        public MultiResizeRule[] Sub { get; set; }


        [PrivateApi("Important for using these settings, but not relevant outside of this")]
        public SrcSetPart[] SrcSetParsed { get; private set; }

        [PrivateApi]
        internal virtual MultiResizeRule InitAfterLoad(double factor, int widthIfEmpty, MultiResizeRule defaultsIfEmpty)
        {
            FactorParsed = factor;
            if (Width == 0) Width = widthIfEmpty;
            SrcSet = SrcSet ?? defaultsIfEmpty?.SrcSet;
            Sizes = Sizes ?? defaultsIfEmpty?.Sizes;
            Media = Media ?? defaultsIfEmpty?.Media;
            SrcSetParsed = SrcSetParser.ParseSet(SrcSet);

            Sub = Sub ?? Array.Empty<MultiResizeRule>();
            foreach (var s in Sub) s?.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);

            return this;
        }
    }
}
