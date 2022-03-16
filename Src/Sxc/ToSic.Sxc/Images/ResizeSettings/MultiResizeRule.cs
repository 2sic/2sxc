using Newtonsoft.Json;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class MultiResizeRule : IMultiResizeRule
    {
        /// <summary>
        /// The resize factor for which this rules is meant.
        /// Used in cases where there are many such rules and the one will be picked that matches this factor.
        /// </summary>
        [JsonProperty("factor")]
        public double Factor { get; set; }

        /// <summary>
        /// The initial width to assume in this resize, from which other sizes would be calculated.
        /// 
        /// If set to 0, it will be ignored. 
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// Source-Set rules (comma separated) which will determine what will be generated.
        ///
        /// Examples:
        /// - `1x,1.5x,2x` - screen resolutions
        /// - `200w,400w,600w,800w,1000w` - pixel sizes
        /// - `0.5*,1*,1.5*,2*` - multipliers of the originally specified pixel size
        /// </summary>
        [JsonProperty("srcset")]
        public string SrcSet { get; set; }

        /// <summary>
        /// Optional `sizes` attribute which would be added to `img` tags
        /// </summary>
        [JsonProperty("sizes")]
        public string Sizes { get; set; }

        /// <summary>
        /// Optional `media` attribute which would be added to `source` tags
        /// </summary>
        [JsonProperty("media")]
        public string Media { get; set; }

        [Eav.Documentation.PrivateApi("Important foc using these settings, but not relevant outside of this")]
        public SrcSetPart[] SrcSetParts { get; private set; }

        [Eav.Documentation.PrivateApi]
        internal virtual MultiResizeRule InitAfterLoad(double factor, int widthIfEmpty, IMultiResizeRule defaultsIfEmpty)
        {
            Factor = factor;
            if (Width == 0) Width = widthIfEmpty;
            SrcSet = SrcSet ?? defaultsIfEmpty?.SrcSet;
            Sizes = Sizes ?? defaultsIfEmpty?.Sizes;
            Media = Media ?? defaultsIfEmpty?.Media;
            SrcSetParts = SrcSetParser.ParseSet(SrcSet);
            return this;
        }
    }
}
