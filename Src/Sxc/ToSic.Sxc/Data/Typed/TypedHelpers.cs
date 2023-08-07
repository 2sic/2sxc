using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Typed
{
    internal class TypedHelpers
    {
        public static IEnumerable<string> FilterKeysIfPossible(string noParamOrder, IEnumerable<string> only, IEnumerable<string> result)
        {
            Protect(noParamOrder, nameof(only), methodName: nameof(IHasKeys.Keys));
            if (result == null) return Array.Empty<string>();

            if (only == default || !only.Any()) return result;
            var filtered = result.Where(r => only.Any(k => k.EqualsInsensitive(r))).ToArray();
            return filtered;
        }

    }
}
