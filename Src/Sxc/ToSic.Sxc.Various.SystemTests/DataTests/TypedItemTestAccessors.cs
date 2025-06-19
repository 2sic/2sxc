using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests;

internal static class TypedItemTestAccessors
{
    public static ITypedMetadata TestMetadata(this ITypedItem item) => item.Metadata;

    public static bool TestContainsKey(this ITyped item, string key) => item.ContainsKey(key);

    public static IEnumerable<string> TestKeys(this ITyped item, IEnumerable<string> only = default) => item.Keys(only: only);
}