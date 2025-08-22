using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys.VaryBy;
internal class CacheVaryByModelHelper
{
    internal static List<KeyValuePair<string, string>> VaryByModelExtract(IDictionary<string, object?> model, string[] nameList)
    {
        var all = model
            .Where(pair => nameList.Any(n
                    => n.EqualsInsensitive(pair.Key)                    // contains key
                       && IsUsefulForCacheKey(pair.Value)     // is simple value, allowing use in cache key
            ))
            .Select(p => new KeyValuePair<string, string>(p.Key, p.Value?.ToString() ?? ""))
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ToList();
        return all;
    }

    internal static bool IsUsefulForCacheKey(object? value)
    {
        if (value == null)
            return false;
        var type = value.GetType().UnboxIfNullable();
        return type.IsValueType
               || type.IsPrimitive
               || type == typeof(string)
               || type == typeof(DateTime)
               || type == typeof(Guid)
               || type.IsNumeric();
    }
}
