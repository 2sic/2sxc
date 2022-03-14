using System;
using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class FactorMapHelper
    {
        public const char ItemSeparator = '\n';
        public const char SrcSetSeparator = ';';
        public const char KeyValueSeparator = SrcSetParser.KeyValueSeparator;

        public static FactorMap[] CreateFromString(string map)
        {
            if (string.IsNullOrWhiteSpace(map)) return Array.Empty<FactorMap>();
            var list = map
                .Split(ItemSeparator)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
            if (list.Length == 0) return Array.Empty<FactorMap>();
            var listSet = list
                .Select(s =>
                {
                    var splitSrcMap = s.Split(SrcSetSeparator);
                    var kvp = splitSrcMap[0].Split(KeyValueSeparator);
                    return new { Rule = s, pair = kvp, SrcMap = splitSrcMap.Length > 1 ? splitSrcMap[1] : "" };
                })
                .Where(r => r.pair.Length == 2)
                .Select(r => new FactorMap
                {
                    Rule = r.Rule,
                    Factor = DoubleOrNullWithCalculation(r.pair[0]) ?? 0,
                    Width = IntOrNull(r.pair[1]) ?? 0,
                    SrcSet = r.SrcMap,
                })
                .Where(fm => fm.Width != 0 && !DNearZero(fm.Factor))
                .ToArray();
            return listSet;
        }

        public static FactorMap Find(FactorMap[] maps, double factor)
        {
            if (maps == null || !maps.Any() || DNearZero(factor)) return null;
            return maps.FirstOrDefault(m => DNearZero(m.Factor - factor));
        }
    }
}
