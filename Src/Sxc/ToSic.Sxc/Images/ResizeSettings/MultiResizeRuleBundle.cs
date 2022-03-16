using System;
using Newtonsoft.Json;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// # BETA
    /// 
    /// Rule how to generate tags etc. which can contain multiple resized images.
    /// In addition to the rule itself, which is seen as the default, for all use cases, it also has additional variations for other use cases.
    /// This way you can specify deviating rules for the `img` tag generation or the `source` tags
    /// </summary>
    [PrivateApi("Still Beta / WIP, not sure if this should be visible")]
    public class MultiResizeRuleBundle: MultiResizeRule, IMultiResizeRuleBundle
    {
        [JsonProperty("img")]
        public MultiResizeRule Img { get; set; }

        [JsonProperty("sources")]
        public MultiResizeRule[] Sources { get; set; }

        internal new MultiResizeRuleBundle InitAfterLoad(double factor, int widthIfEmpty, IMultiResizeRule defaultsIfEmpty)
        {
            base.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);
            (Img as MultiResizeRule)?.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);
            Sources = Sources ?? Array.Empty<MultiResizeRule>();
            foreach (var s in Sources) s?.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);
            return this;
        }

    }
}