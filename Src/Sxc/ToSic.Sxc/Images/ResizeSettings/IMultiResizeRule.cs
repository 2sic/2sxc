using Newtonsoft.Json;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// # BETA
    /// 
    /// Rule how to generate tags etc. which can contain multiple resized images
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta / WIP")]
    public interface IMultiResizeRule
    {
        /// <summary>
        /// The resize factor for which this rules is meant.
        /// Used in cases where there are many such rules and the one will be picked that matches this factor.
        /// </summary>
        [JsonProperty("factor")]
        double Factor { get; }

        /// <summary>
        /// The initial width to assume in this resize, from which other sizes would be calculated.
        /// 
        /// If set to 0, it will be ignored. 
        /// </summary>
        [JsonProperty("width")]
        int Width { get; }

        /// <summary>
        /// Source-Set rules (comma separated) which will determine what will be generated.
        ///
        /// Examples:
        /// - `1x,1.5x,2x` - screen resolutions
        /// - `200w,400w,600w,800w,1000w` - pixel sizes
        /// - `0.5*,1*,1.5*,2*` - multipliers of the originally specified pixel size
        /// </summary>
        [JsonProperty("srcset")]
        string SrcSet { get; }

        /// <summary>
        /// Optional `sizes` attribute which would be added to `img` tags
        /// </summary>
        [JsonProperty("sizes")]
        string Sizes { get; }

        /// <summary>
        /// Optional `media` attribute which would be added to `source` tags
        /// </summary>
        [JsonProperty("media")]
        string Media { get; }

        [PrivateApi]
        SrcSetPart[] SrcSetParts { get; }
    }
}