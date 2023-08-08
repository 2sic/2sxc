using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Typed
{
    internal static class TypedHelpers
    {
        public static IEnumerable<string> FilterKeysIfPossible(string noParamOrder, IEnumerable<string> only, IEnumerable<string> result)
        {
            Protect(noParamOrder, nameof(only), methodName: nameof(IHasKeys.Keys));
            if (result == null) return Array.Empty<string>();

            if (only == default || !only.Any()) return result;
            var filtered = result.Where(r => only.Any(k => k.EqualsInsensitive(r))).ToArray();
            return filtered;
        }

        public static bool IsErrStrict(bool found, bool? required, bool requiredDefault)
            => !found && (required ?? requiredDefault);


        public static bool IsErrStrict(ITyped parent, string name, bool? required, bool requiredDefault)
            => !parent.ContainsKey(name) && (required ?? requiredDefault);

        public static ArgumentException ErrStrict(string name, [CallerMemberName] string cName = default)
        {
            var help = $"Either a) correct the name '{name}'; b) use {cName}(\"{name}\", required: false); or c) or use AsItem(..., strict: false) or AsTyped(..., strict: false)";
            var msg = cName == "."
                ? $".{name} not found and 'strict' is true, meaning that an error is thrown. {help}"
                : $"{cName}('{name}', ...) not found and 'strict' is true, meaning that an error is thrown. {help}";
            return new ArgumentException(msg, nameof(name));
        }

    }
}
