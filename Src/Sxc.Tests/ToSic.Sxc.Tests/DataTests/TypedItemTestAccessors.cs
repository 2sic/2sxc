using System.Collections.Generic;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests;

internal static class TypedItemTestAccessors
{
    public static IMetadata TestMetadata(this ITypedItem item) => item.Metadata;

    public static bool TestContainsKey(this ITyped item, string key) => item.ContainsKey(key);

    public static IEnumerable<string> TestKeys(this ITyped item, IEnumerable<string> only = default) => item.Keys(only: only);
}