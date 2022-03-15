using Newtonsoft.Json;
using ToSic.Eav.Documentation;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// Settings which can apply to an img, source tag or both
    /// </summary>
    public class ResizeSettingsSrcSet
    {
        [JsonProperty("factor")]
        public double Factor { get; set; }

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

        public SrcSetPart[] SrcSetParts { get; private set; }

        [PrivateApi]
        internal virtual ResizeSettingsSrcSet InitAfterLoad(double factor, int widthIfEmpty, ResizeSettingsSrcSet defaultsIfEmpty)
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
