using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Data.Typed
{
    internal static class TypedHelpers
    {
        public static bool ContainsKey<TNode>(string name, TNode start, Func<TNode, string, bool> checkNode, Func<TNode, string, TNode> dig) where TNode: class
        {
            var parts = PropertyStack.SplitPathIntoParts(name);
            if (!parts.Any()) return false;

            var current = start;
            var max = parts.Length - 1;
            for (var i = 0; i < parts.Length; i++)
            {
                var key = parts[i];
                var has = checkNode(current, key); // current.Attributes.ContainsKey(key);
                if (i == max || !has) return has;

                // has = true, and we have more nodes, so we must check the children
                //var children = current.Children(key);
                //if (!children.Any()) return false;
                current = dig(current, key); // children[0];
                if (current == null) return false;
            }

            return false;

        }

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
