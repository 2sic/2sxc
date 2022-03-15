using Newtonsoft.Json;
using ToSic.Eav.Documentation;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    [PrivateApi("WIP")]
    public class FactorRule
    {
        public string Rule { get; set; }

        [JsonProperty(PropertyName = "factor")]
        public double Factor { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName = "srcset")]
        public string SrcSet { get; set; }

        public SrcSetPart[] SrcSetParts => _srcSetParts ?? (_srcSetParts = SrcSetParser.ParseSet(SrcSet));
        private SrcSetPart[] _srcSetParts;
    }
}
