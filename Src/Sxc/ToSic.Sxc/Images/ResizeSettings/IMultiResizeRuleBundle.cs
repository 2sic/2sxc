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
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta / WIP, not sure if this should be visible")]
    public interface IMultiResizeRuleBundle
    {
        [JsonProperty("img")]
        MultiResizeRule Img { get; set; }

        [JsonProperty("sources")]
        MultiResizeRule[] Sources { get; set; }
    }
}