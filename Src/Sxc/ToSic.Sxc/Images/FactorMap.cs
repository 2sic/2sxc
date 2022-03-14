using ToSic.Eav.Documentation;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    [PrivateApi("WIP")]
    public class FactorMap
    {
        public string Rule { get; set; }
        public double Factor { get; set; }
        public int Width { get; set; }

        public string SrcSet { get; set; }

        public SrcSetPart[] SrcSetParts => _srcSetParts ?? (_srcSetParts = SrcSetParser.ParseSet(SrcSet));
        private SrcSetPart[] _srcSetParts;
    }
}
