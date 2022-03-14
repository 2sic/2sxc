using System;
using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class FactorMapHelper
    {
        public const char FactorSeparator = '=';

        public static FactorMap[] CreateFromString(string map)
        {
            if (string.IsNullOrWhiteSpace(map)) return Array.Empty<FactorMap>();
            var list = map
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
            if (list.Length == 0) return Array.Empty<FactorMap>();
            var listSet = list
                .Select(s => (Rule: s, pair: s.Split(FactorSeparator)))
                .Where(r => r.pair.Length == 2)
                .Select(r => new FactorMap
                {
                    Rule = r.Rule,
                    Factor = DoubleOrNullWithCalculation(r.pair[0]) ?? 0,
                    Width = IntOrNull(r.pair[1]) ?? 0
                })
                .Where(fm => fm.Width != 0 && !DNearZero(fm.Factor))
                .ToArray();
            return listSet;
        }

        public static int? Width(FactorMap[] map, double factor, int original)
        {
            if (map == null || !map.Any() || original == 0 || DNearZero(factor)) return null;
            var found = map.FirstOrDefault(m => DNearZero(m.Factor - factor));
            // The default would have 0 on factor and null on rule
            return found.Factor != 0 ? found.Width : (int?)null;
        }
    }
}
